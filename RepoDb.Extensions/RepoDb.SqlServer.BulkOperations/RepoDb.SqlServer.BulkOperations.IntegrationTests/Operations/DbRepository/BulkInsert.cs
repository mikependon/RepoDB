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
    public class DbRepositoryOperationsTest
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
        public void TestDbRepositoryBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertForEntitiesIfTheMappingsAreInvalid()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.BulkInsert(tables, mappings);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsert<BulkInsertIdentityTable>((DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesDbDataReaderWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsert<BulkInsertIdentityTable>((DbDataReader)reader, mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationRepository.BulkInsert<BulkInsertIdentityTable>((DbDataReader)reader, mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsert<BulkInsertIdentityTable>(table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesDataTableWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsert<BulkInsertIdentityTable>(table, DataRowState.Unchanged, mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationRepository.BulkInsert<BulkInsertIdentityTable>(table, DataRowState.Unchanged, mappings);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesWithExtraFieldsWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

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
        public void TestDbRepositoryBulkInsertForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsert(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), (DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsert(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables, mappings);

                // Trigger
                var result = bulkInsertResult.Result;
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>((DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesDbDataReaderWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>((DbDataReader)reader, mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>((DbDataReader)reader, mappings);

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>(table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesDataTableWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>(table, DataRowState.Unchanged, mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsertAsync<BulkInsertIdentityTable>(table, DataRowState.Unchanged, mappings);

                            // Trigger
                            var result = bulkInsertResult.Result;
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

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
        public void TestDbRepositoryBulkInsertAsyncForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll<BulkInsertIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkInsertIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationRepository.BulkInsertAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), (DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationRepository.BulkInsertAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationRepository.QueryAll<BulkInsertIdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        #endregion
    }
}
