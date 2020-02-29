using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class SystemSqlConnectionDbRepositoryBulkDeleteOperationsTest
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

        #region BulkDelete<TEntity>

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDelete<BulkOperationIdentityTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables,
                    usePhysicalPseudoTempTable: true);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.BulkDelete(tables, mappings: mappings);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDbDataReaderWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader,
                             mappings: mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationRepository.BulkDelete<BulkOperationIdentityTable>((DbDataReader)reader,
                             mappings: mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDelete<BulkOperationIdentityTable>(table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDataTableWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDelete<BulkOperationIdentityTable>(table,
                                rowState: DataRowState.Unchanged,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationRepository.BulkDelete<BulkOperationIdentityTable>(table,
                                rowState: DataRowState.Unchanged,
                                mappings: mappings);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkDelete<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForEntitiesWithExtraFieldsWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDelete(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys: primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkDeleteAsync<TEntity>

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync<BulkOperationIdentityTable>(primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables,
                    usePhysicalPseudoTempTable: true).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables,
                    mappings: mappings);

                // Trigger
                var result = bulkDeleteResult.Result;
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDbDataReaderWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>((DbDataReader)reader,
                            mappings: mappings);

                        // Trigger
                        var result = bulkDeleteResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>(table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDataTableWithMappings()
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
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>(table,
                                rowState: DataRowState.Unchanged,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDeleteAsync<BulkOperationIdentityTable>(table,
                                rowState: DataRowState.Unchanged,
                                mappings: mappings);

                            // Trigger
                            var result = bulkDeleteResult.Result;
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkDeleteAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForEntitiesWithExtraFieldsWithMappings()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDeleteAsync(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys: primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[BulkOperationIdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkDeleteResult = destinationRepository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), (DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkDeleteResult);

                        // Act
                        var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                        // Assert
                        Assert.AreEqual(0, countResult);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                repository.InsertAll(tables);
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
                        using (var destinationRepository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkDeleteResult = destinationRepository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationRepository.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
