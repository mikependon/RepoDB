using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class SqlConnectionBulkUpdateOperationsTest
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
        public void TestSqlConnectionBulkUpdateForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.BulkInsert(tables);

                // Setup
                tables.ForEach(t =>
                {
                    t.ColumnBit = !t.ColumnBit;
                    t.ColumnDateTime = DateTime.UtcNow.Date;
                    t.ColumnDateTime2 = DateTime.UtcNow;
                    t.ColumnDecimal = Convert.ToDecimal(new Random().Next());
                    t.ColumnFloat = Convert.ToSingle(new Random().Next());
                    t.ColumnInt = new Random().Next();
                    t.ColumnNVarChar = $"{t.ColumnNVarChar}-UPDATED";
                });

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForEntitiesIfTheMappingsAreInvalid()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.BulkUpdate(tables, null, mappings);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkInsertIdentityTable>((DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate<BulkInsertIdentityTable>((DbDataReader)reader, null, mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkUpdate<BulkInsertIdentityTable>((DbDataReader)reader, null, mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate<BulkInsertIdentityTable>(table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate<BulkInsertIdentityTable>(table, null, DataRowState.Unchanged, mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count, queryResult.Count());
                            tables.AsList().ForEach(t =>
                            {
                                Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                            });
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForEntitiesDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkUpdate<BulkInsertIdentityTable>(table, null, DataRowState.Unchanged, mappings);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkUpdate<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForEntitiesWithExtraFieldsWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

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
        public void TestSqlConnectionBulkUpdateForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), (DbDataReader)reader);
                        
                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                            (DbDataReader)reader,
                            null,
                            mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                            (DbDataReader)reader,
                            null,
                            mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkUpdate("InvalidTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkUpdate("MissingTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged,
                                mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged,
                                mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkUpdate("InvalidTable",
                                table,
                                null,
                                DataRowState.Unchanged);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkUpdate("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkUpdateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForEntitiesIfTheMappingsAreInvalid()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    null,
                    mappings);

                // Trigger
                var result = bulkUpdateResult.Result;
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>((DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>((DbDataReader)reader,
                            null,
                            mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>((DbDataReader)reader,
                            null,
                            mappings);

                        // Trigger
                        var result = bulkUpdateResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>(table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>(table,
                                null,
                                DataRowState.Unchanged,
                                mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync<BulkInsertIdentityTable>(table,
                                null,
                                DataRowState.Unchanged,
                                mappings);

                            // Trigger
                            var result = bulkUpdateResult.Result;
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkUpdateAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForEntitiesWithExtraFieldsWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

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
        public void TestSqlConnectionBulkUpdateAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), (DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                            (DbDataReader)reader,
                            null,
                            mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkUpdateResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                            (DbDataReader)reader,
                            null,
                            mappings);

                        // Trigger
                        var result = bulkUpdateResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync("InvalidTable", (DbDataReader)reader);

                        // Trigger
                        var result = bulkUpdateResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkUpdateResult = destinationConnection.BulkUpdateAsync("MissingTable", (DbDataReader)reader);

                        // Trigger
                        var result = bulkUpdateResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkUpdateAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.RowGuid), nameof(BulkInsertIdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged,
                                mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged,
                                mappings);

                            // Trigger
                            var result = bulkUpdateResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync("InvalidTable", table);

                            // Trigger
                            var result = bulkUpdateResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged);

                            // Trigger
                            var result = bulkUpdateResult.Result;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
