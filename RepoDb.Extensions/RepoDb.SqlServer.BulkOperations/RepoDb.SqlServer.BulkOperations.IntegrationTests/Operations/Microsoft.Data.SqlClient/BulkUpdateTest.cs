using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class MicrosoftSqlConnectionBulkUpdateOperationsTest
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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical);

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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical);

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
        public void TestMicrosoftSqlConnectionBulkUpdateForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<InvalidOperationException>(() => connection.BulkUpdate(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader,
                            mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader,
                            mappings: mappings));
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullEntities()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForEmptyEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionString))
        //    {
        //        connection.BulkUpdate(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullDataReader()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DbDataReader)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForNullDataTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkUpdate<TEntity>(Extra Fields)

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkUpdateForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
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
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical);

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
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            reader,
                            mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            reader,
                            mappings: mappings));
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable", reader));
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable", reader));
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
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
        public void TestMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidOperationException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical).Result;

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical).Result;

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(tables,
                    mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader,
                            mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader,
                            mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullEntities()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync((IEnumerable<BulkOperationIdentityTable>)null, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Wait());  // TODO: Remove the 'pseudoTableType' in the future
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForEmptyEntities()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(Enumerable.Empty<BulkOperationIdentityTable>(), pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Wait();)  // TODO: Remove the 'pseudoTableType' in the future
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullDataReader()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DbDataReader)null, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Wait());  // TODO: Remove the 'pseudoTableType' in the future
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForNullDataTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Wait());  // TODO: Remove the 'pseudoTableType' in the future
            }
        }

        #endregion

        #region BulkUpdateAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), entities, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBulkUpdateAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical).Result;

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
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            reader,
                            mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                            reader,
                            mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable", reader, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                    {
                        // Act
                        Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable", reader, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result;  // TODO: Remove the 'pseudoTableType' in the future

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<SqlServerBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new SqlServerBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable", table, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMicrosoftSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable",
                                table, pseudoTableType: Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto).Result);  // TODO: Remove the 'pseudoTableType' in the future
                        }
                    }
                }
            }
        }

        #endregion
    }
}
