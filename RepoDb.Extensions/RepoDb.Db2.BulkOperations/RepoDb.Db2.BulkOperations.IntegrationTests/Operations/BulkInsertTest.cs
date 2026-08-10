using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations.Db2;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Db2.BulkOperations.IntegrationTests.Models;

namespace RepoDb.Db2.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class DB2ConnectionBulkInsertOperationsTest
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
        public void TestDB2ConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<InvalidTypeException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
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
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNullEntities()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnDB2ConnectionBulkInsertForEmptyEntities()
        //{
        //    using (var connection = new DB2Connection(Database.ConnectionString))
        //    {
        //        connection.BulkInsert(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNullDataTable()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkInsert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForTableNameDbDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
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
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
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
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNullEntities()
        //{
        //    using (var connection = new DB2Connection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForEmptyEntities()
        //{
        //    using (var connection = new DB2Connection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNullDataTable()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNullEntities()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Result);
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDbDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<InvalidTypeException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityNullEntities()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityNullDataTable()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityNullEntities()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Result);
            }
        }

        [TestMethod]
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: Db2BulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());

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
        public void TestDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
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
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<Db2BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new Db2BulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnDB2ConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                        {
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
        public void TestDB2ConnectionBulkInsertForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                {
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
        public void TestDB2ConnectionBulkInsertAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new DB2Connection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                using (var destinationConnection = new DB2Connection(Database.ConnectionString))
                {
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
