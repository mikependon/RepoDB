using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkInsertOperationsTest
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
        public void TestFirebirdConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void TestFirebirdConnectionBulkInsertForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<FormatException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForEntitiesDataTable()
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
        public void TestFirebirdConnectionBulkInsertForEntitiesDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNullEntities()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnFirebirdConnectionBulkInsertForEmptyEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        connection.BulkInsert(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkInsert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForEntitiesWithExtraFieldsWithMappings()
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
        public void TestFirebirdConnectionBulkInsertForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForTableNameDbDataTable()
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
        public void TestFirebirdConnectionBulkInsertForTableNameDbDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForTableNameDataTable()
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
        public void TestFirebirdConnectionBulkInsertForTableNameDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void TestFirebirdConnectionBulkInsertAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesDataTable()
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
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNullEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForEmptyEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNullEntities()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Result);
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDataTable()
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDbDataTable()
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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDbDataTableWithReturnIdentity()
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
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region NonIdentityTable Mirrors

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<FormatException>(() => connection.BulkInsert(tables, mappings));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityNullEntities()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkInsertForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<FormatException>(() => destinationConnection.BulkInsert(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(tables, mappings).Result);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableWithMappings()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameExpandoObjectsWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityNullEntities()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Result);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTable()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableWithReturnIdentity()
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));

            // Add the mappings
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

                        foreach (DataRow row in table.Rows)
                        {
                            row["Id"] = Convert.ToInt64(row["Id"]) + 100000;
                        }

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("InvalidTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkInsertAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsert(DbDataReader)

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkInsertAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
