using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class DbRepositoryOperationsTest
    {
        private const int BatchQueryFirstPage = 0;

        private const int BatchQuerySecondPage = 1;

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

        #region BatchQuery

        [TestMethod]
        public void TestDbRepositoryBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null);

                // Assert (0, 3)
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null);

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQuerySecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null);

                // Assert (4, 7)
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQuerySecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null);

                // Assert (5, 2)
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryForDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryForQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryForQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryForQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (0, 3)
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (4, 7)
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (5, 2)
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnInt)));

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
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        repository.BulkInsert<IdentityTable>((DbDataReader)reader);

                        // Act
                        var result = repository.QueryAll<IdentityTable>();

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
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.RowGuid), nameof(IdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        repository.BulkInsert<IdentityTable>((DbDataReader)reader, mappings);

                        // Act
                        var result = repository.QueryAll<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        repository.BulkInsert<IdentityTable>((DbDataReader)reader, mappings);
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result);

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result);

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnInt)));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables, mappings);
                bulkInsertResult.Wait();

                // Trigger
                var result = bulkInsertResult.Result;
            }
        }

        [TestMethod]
        public void TestDbRepositoryBulkInsertAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = repository.BulkInsertAsync<IdentityTable>((DbDataReader)reader);
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = repository.QueryAll<IdentityTable>();

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
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.RowGuid), nameof(IdentityTable.RowGuid)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = repository.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings);
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = repository.QueryAll<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnInt)));

            // Insert the records first
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => repository.Insert(t));
            }

            // Open the source repository
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source repository
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination repository
                    using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = repository.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings);
                        bulkInsertResult.Wait();

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        #endregion

        #region Count

        [TestMethod]
        public void TestDbRepositoryCountViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count<IdentityTable>(field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync<IdentityTable>(field).Result;

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestDbRepositoryCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbRepositoryCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestDbRepositoryDeleteWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaDelete()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(new { last.Id });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(c => c.ColumnInt == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>(c => c.ColumnInt == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>(field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestDbRepositoryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.DeleteAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.DeleteAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestDbRepositoryInsert()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertForIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = repository.Insert<IdentityTable, long>(item);

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertForNonIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = repository.Insert<NonIdentityTable, Guid>(item);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbRepositoryInsertAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.InsertAsync(item).Result));

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncForIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = repository.InsertAsync<IdentityTable, long>(item).Result;

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncForNonIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = repository.InsertAsync<NonIdentityTable, Guid>(item).Result;

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestDbRepositoryMerge()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult);

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithPrimaryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new Field(nameof(IdentityTable.Id)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new Field(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new[]
                {
                    new Field(nameof(IdentityTable.ColumnInt)),
                    new Field(nameof(IdentityTable.ColumnBit))
                });

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, Field.From(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnBit)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbRepositoryMergeAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithPrimaryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new Field(nameof(IdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new Field(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new[]
                {
                    new Field(nameof(IdentityTable.ColumnInt)),
                    new Field(nameof(IdentityTable.ColumnBit))
                }).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.EpocDate;
                queryResult.ColumnDateTime2 = Helper.EpocDate;
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, Field.From(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnBit))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.EpocDate, queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestDbRepositoryQueryWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(whereOrPrimaryKey: null,
                    top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(fields, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 5),
                new QueryField(nameof(IdentityTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryGroupWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(queryGroup, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == false);

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => !c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
                    top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.Id == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(4, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields, top: top).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 5),
                new QueryField(nameof(IdentityTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup, top: top).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == false).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestDbRepositoryQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new
            {
                Id = Order.Ascending
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAll<IdentityTable>(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithHint()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAll<IdentityTable>(hints: SqlTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderByAndWithHint()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new
            {
                Id = Order.Ascending
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAll<IdentityTable>(orderBy: orderBy,
                    hints: SqlTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbRepositoryQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new
            {
                Id = Order.Ascending
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithHint()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(hints: SqlTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderByAndWithHint()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new
            {
                Id = Order.Ascending
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(orderBy: orderBy,
                    hints: SqlTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryMultiple

        #region QueryMultiple<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5, T6>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5, T6, T7>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6,
                    where7: item => item.ColumnInt == 7);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.Item7.First());
            }
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region QueryMultipleAsync<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5, T6>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6,
                    where7: item => item.ColumnInt == 7).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.Item7.First());
            }
        }

        #endregion

        #endregion

        #region Truncate

        [TestMethod]
        public void TestDbRepositoryTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                repository.Truncate<IdentityTable>();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbRepositoryTruncateAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var task = repository.TruncateAsync<IdentityTable>();
                task.Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbRepositoryUpdateViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.Update(item, item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.Update(item, new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.Update(item, c => c.Id == item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(last, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = repository.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(last, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = repository.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(last, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = repository.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item, item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item, new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item, c => c.Id == item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = repository.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = repository.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = repository.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestDbRepositoryExecuteQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<LiteIdentityTable>("SELECT * FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.Where(t => t.Id == item.Id).First();
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new QueryField("Id", Operation.NotEqual, 1);

                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<LiteIdentityTable>("SELECT * FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.Where(t => t.Id == item.Id).First();
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new QueryField("Id", Operation.NotEqual, 1);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithNoAffectedTableRows()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteNonQuery("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;");

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryDeleteSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteNonQuery("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithNoAffectedTableRows()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteNonQueryAsync("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;").Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100;").Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 1;").Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteNonQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithoutRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithSingleRowAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithMultipleRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithoutRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithSingleRowAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1 AS Value1, 2 AS Value2;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1, @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalarAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar<T>

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithoutRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithSingleRowAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithMultipleRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalar<int>("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar<object>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar<object>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync<T>

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithoutRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithSingleRowAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithMultipleRowsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 1 AS Value1, 2 AS Value2;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1, @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalarAsync<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarTAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion
    }
}
