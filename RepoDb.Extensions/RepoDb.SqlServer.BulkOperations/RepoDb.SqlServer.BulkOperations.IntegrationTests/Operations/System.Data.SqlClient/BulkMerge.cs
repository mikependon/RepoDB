using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlClient;
using RepoDb.Exceptions;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class SystemSqlConnectionBulkMergeOperationsTest
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
        public void TestSystemSqlConnectionBulkMergeForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    usePhysicalPseudoTempTable: true);

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
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    usePhysicalPseudoTempTable: true);

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
        public void TestSystemSqlConnectionBulkMergeForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForEntitiesIfTheMappingsAreInvalid()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.BulkMerge(tables, mappings: mappings);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkMerge<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge<BulkOperationIdentityTable>(table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForEntitiesDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkMerge<BulkOperationIdentityTable>(table,
                                mappings: mappings);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(NullReferenceException))]
        //public void ThrowExceptionOnSystemSqlConnectionBulkMergeForNullEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        connection.BulkMerge((IEnumerable<BulkOperationIdentityTable>)null);
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnSystemSqlConnectionBulkMergeForEmptyEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        connection.BulkMerge(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForNullDataReader()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DbDataReader)null);
            }
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForNullDataTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null);
            }
        }

        #endregion

        #region BulkMerge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForEntitiesWithExtraFieldsWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true);
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            (DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            (DbDataReader)reader,
                            mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            (DbDataReader)reader,
                            mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkMerge("InvalidTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkMerge("MissingTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true);

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
        public void TestSystemSqlConnectionBulkMergeForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkMerge("InvalidTable",
                                table);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkMerge("MissingTable",
                                table);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    usePhysicalPseudoTempTable: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    usePhysicalPseudoTempTable: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForEntitiesIfTheMappingsAreInvalid()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    mappings: mappings);

                // Trigger
                var result = bulkMergeResult.Result;
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings);

                        // Trigger
                        var result = bulkMergeResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync<BulkOperationIdentityTable>(table,
                                mappings: mappings);

                            // Trigger
                            var result = bulkMergeResult.Result;
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForNullEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForEmptyEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
        //    }
        //}

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForNullDataReader()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DbDataReader)null).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForNullDataTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait();
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, isReturnIdentity: true).Result;
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            (DbDataReader)reader,
                            mappings: mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkMergeResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            (DbDataReader)reader,
                            mappings: mappings);

                        // Trigger
                        var result = bulkMergeResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync("InvalidTable", (DbDataReader)reader);

                        // Trigger
                        var result = bulkMergeResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkMergeResult = destinationConnection.BulkMergeAsync("MissingTable", (DbDataReader)reader);

                        // Trigger
                        var result = bulkMergeResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, isReturnIdentity: true).Result;

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
        public void TestSystemSqlConnectionBulkMergeAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Trigger
                            var result = bulkMergeResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync("InvalidTable", table);

                            // Trigger
                            var result = bulkMergeResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync("MissingTable",
                                table);

                            // Trigger
                            var result = bulkMergeResult.Result;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
