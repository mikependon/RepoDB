using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class ClickHouseConnectionBulkInsertOperationsTest
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

        #region BulkInsert<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertForNonIdentityEntities()
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

                foreach (var table in tables)
                {
                    table.Id = table.Id + 100000;
                }

                // Open the destination connection
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(tables));
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertForNonIdentityEntitiesDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            Assert.Throws<AggregateException>(() =>
                                destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<AggregateException>(() =>
                    connection.BulkInsert(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<InvalidTypeException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForEntitiesDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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

                            // The mappings above deliberately omit Id (ClickHouse has no auto-generated
                            // identity to fall back to for an unmapped sort-key column), so every row bulk-
                            // inserted below lands with Id = 0, colliding with every other newly-inserted row
                            // (not with the pre-existing 1..10 rows, which don't collide with each other or
                            // with 0). Without pausing the background merge scheduler, those 10 same-Id(0)
                            // rows can be deduped down to as few as 1 by the time QueryAll below runs, non-
                            // deterministically undercounting - see Helper.StopMerges's remarks.
                            Helper.StopMerges(destinationConnection, "BulkOperationIdentityTable");
                            try
                            {
                                // Act
                                var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings);

                                // Assert
                                Assert.AreEqual(tables.Count, bulkInsertResult);

                                // Act
                                var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                                // Assert
                                Assert.AreEqual(tables.Count * 2, queryResult.Count());
                            }
                            finally
                            {
                                Helper.StartMerges(destinationConnection, "BulkOperationIdentityTable");
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkInsertForEmptyEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        connection.BulkInsert(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkInsert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForEntitiesWithExtraFieldsWithMappings()
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

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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

        #region BulkInsert(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForTableNameDbDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForTableNameDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsertAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<AggregateException>(async () =>
                    await connection.BulkInsertAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForEntitiesDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNullEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForEmptyEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Result);
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
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

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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

        #region BulkInsertAsync(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameDbDataTable()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region NonIdentityTable Mirrors

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsert(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<InvalidTypeException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntitiesDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

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

                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameDbDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
                var bulkInsertResult = connection.BulkInsertAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableWithMappings()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertAsyncForNonIdentityEntities()
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

                foreach (var table in tables)
                {
                    table.Id = table.Id + 100000;
                }

                // Open the destination connection
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    Assert.Throws<AggregateException>(async () => await destinationConnection.BulkInsertAsync(tables));
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            Assert.Throws<AggregateException>(async () =>
                                await destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityNullDataTable()
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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

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

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

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
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsert(DbDataReader)

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    // Act
                    var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);

                    // Act
                    var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count * 2, queryResult.Count());
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkInsertAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    // Act
                    var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), reader).Result;

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);

                    // Act
                    var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count * 2, queryResult.Count());
                }
            }
        }

        #endregion

    }
}
