using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class BaseRepositoryOperationsTest
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

        #region SubClass

        private class SimpleTableRepository : BaseRepository<SimpleTable, SqlConnection>
        {
            public SimpleTableRepository() :
                base(Database.ConnectionStringForRepoDb)
            { }
        }

        private class LiteSimpleTableRepository : BaseRepository<LiteSimpleTable, SqlConnection>
        {
            public LiteSimpleTableRepository() :
                base(Database.ConnectionStringForRepoDb)
            { }
        }

        #endregion

        #region Helper

        private DateTime EpocDate => Helper.GetEpocDate();

        private List<SimpleTable> CreateSimpleTables(int count)
        {
            var tables = new List<SimpleTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new SimpleTable
                {
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = EpocDate.AddDays(index),
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        private void AssertPropertiesEquality<T>(T t1, T t2)
        {
            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                if (p.Name == "Id")
                {
                    return;
                }
                var value1 = p.GetValue(t1);
                var value2 = p.GetValue(t2);
                Assert.AreEqual(value1, value2, $"Assert failed for '{p.Name}'. The values are '{value1}' and '{value2}'.");
            });
        }

        private const int BatchQueryFirstPage = 0;

        private const int BatchQuerySecondPage = 1;

        #endregion

        #region BatchQuery

        [TestMethod]
        public void TestBaseRepositoryBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (0, 3)
                AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQuerySecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQuerySecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryForDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (2)
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryForQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryForQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryForQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQuery(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (0, 3)
                AssertPropertiesEquality(tables.ElementAt(0), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncForDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (2)
                AssertPropertiesEquality(tables.ElementAt(2), result.Extract().ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncForQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(3), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncForQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncForQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.BatchQueryAsync(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Extract().ElementAt(3));
            }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestBaseRepositoryBulkInsertForEntities()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Act
                var queryResult = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnNVarChar)));

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Act
                var queryResult = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnInt)));

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.BulkInsert(tables, mappings);
            }
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestBaseRepositoryBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result.Extract());

                // Act
                var queryResult = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnNVarChar)));

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result.Extract());

                // Act
                var queryResult = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnInt)));

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables, mappings);
                bulkInsertResult.Wait();

                // Trigger
                var result = bulkInsertResult.Result.Extract();
            }
        }

        #endregion

        #region Count

        [TestMethod]
        public void TestBaseRepositoryCount()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count();

                // Assert
                Assert.AreEqual((long)tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual((long)7, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual((long)1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count(field);

                // Assert
                Assert.AreEqual((long)5, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count(fields);

                // Assert
                Assert.AreEqual((long)3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.Count(queryGroup);

                // Assert
                Assert.AreEqual((long)3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestBaseRepositoryCountAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync().Result;

                // Assert
                Assert.AreEqual((long)tables.Count, result.Extract());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual((long)7, result.Extract());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual((long)1, result.Extract());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(field).Result;

                // Assert
                Assert.AreEqual((long)5, result.Extract());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(fields).Result;

                // Assert
                Assert.AreEqual((long)3, result.Extract());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(queryGroup).Result;

                // Assert
                Assert.AreEqual((long)3, result.Extract());
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestBaseRepositoryDeleteWithoutCondition()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual((long)0, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(c => c.ColumnInt == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(new QueryField(nameof(SimpleTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual((long)6, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual((long)6, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaDataEntity()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Delete(last);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.Count());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Extract());
                Assert.AreEqual((long)0, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Extract());
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(c => c.ColumnInt == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Extract());
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), 6);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(field).Result;

                // Assert
                Assert.AreEqual(1, result.Extract());
                Assert.AreEqual((long)tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(fields).Result;

                // Assert
                Assert.AreEqual(4, result.Extract());
                Assert.AreEqual((long)6, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result.Extract());
                Assert.AreEqual((long)6, repository.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(last).Result;

                // Assert
                Assert.AreEqual(1, result.Extract());
                Assert.AreEqual(tables.Count - 1, repository.Count());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestBaseRepositoryDeleteAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.DeleteAll();

                // Assert
                Assert.AreEqual((long)tables.Count, result);
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestBaseRepositoryDeleteAllAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.DeleteAllAsync().Result;

                // Assert
                Assert.AreEqual((long)tables.Count, result.Extract());
            }
        }

        #endregion

        #region InlineInsert

        [TestMethod]
        public void TestBaseRepositoryInlineInsert()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = Convert.ToInt32(repository.InlineInsert(entity));

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = repository.Query();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionAtSqlConnectionInlineInsertIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.InlineInsert(entity);
            }
        }

        #endregion

        #region InlineInsertAsync

        [TestMethod]
        public void TestBaseRepositoryInlineInsertAsync()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = Convert.ToInt32(repository.InlineInsertAsync(entity).Result.Extract());

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = repository.Query();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionAtSqlConnectionInlineInsertAsyncIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.InlineInsertAsync(entity).Result.Extract();
            }
        }

        #endregion

        #region InlineMerge

        [TestMethod]
        public void TestBaseRepositoryInlineMergeWithEmptyTables()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = Convert.ToInt32(repository.InlineMerge(entity, Field.From(nameof(SimpleTable.Id))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineMergeToExistingData()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineMerge(entity, Field.From(nameof(SimpleTable.ColumnInt))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == entity.ColumnInt);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeIfThereAreNoPrimaryKeyAndNoQualifierFieldsDefined()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.InlineMerge(entity);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeIfTheFieldsAreMissingAtTheDataEntityProperties()
        {
            // Setup
            var entity = new
            {
                Id = 1,
                NotAField = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.InlineMerge(entity, Field.From(nameof(SimpleTable.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeIfTheQualifierFieldsAreMissingAtTheObjectProperties()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.InlineMerge(entity, Field.From(nameof(SimpleTable.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowErrorAtSqlConnectionInlineMergeIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = "Invalid"
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.InlineMerge(entity, Field.From(nameof(SimpleTable.Id)));
            }
        }

        #endregion

        #region InlineMergeAsync

        [TestMethod]
        public void TestBaseRepositoryInlineMergeAsyncWithEmptyTables()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = Convert.ToInt32(repository.InlineMergeAsync(entity, Field.From(nameof(SimpleTable.Id))).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineMergeAsyncToExistingData()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineMergeAsync(entity, Field.From(nameof(SimpleTable.ColumnInt))).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == entity.ColumnInt);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeAsyncIfThereAreNoPrimaryKeyAndNoQualifierFieldsDefined()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.InlineMergeAsync(entity).Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeAsyncIfTheFieldsAreMissingAtTheDataEntityProperties()
        {
            // Setup
            var entity = new
            {
                Id = 1,
                NotAField = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.InlineMergeAsync(entity, Field.From(nameof(SimpleTable.Id))).Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowErrorAtSqlConnectionInlineMergeAsyncIfTheQualifierFieldsAreMissingAtTheObjectProperties()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.InlineMergeAsync(entity, Field.From(nameof(SimpleTable.Id))).Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowErrorAtSqlConnectionInlineMergeAsyncIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = "Invalid"
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.InlineMergeAsync(entity, Field.From(nameof(SimpleTable.Id))).Result.Extract();
            }
        }

        #endregion

        #region InlineUpdate

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaPrimaryKey()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, last.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaDynamic()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, new { last.Id }));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaExpression()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, e => e.Id == last.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaQueryField()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, new QueryField(nameof(SimpleTable.Id), last.Id)));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaQueryFields()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 10),
                new QueryField(nameof(SimpleTable.ColumnBit), true)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, fields));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                fields.ResetAll();
                var queryResult = repository.Query(fields);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnDecimal);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateViaQueryGroup()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 10),
                new QueryField(nameof(SimpleTable.ColumnBit), true)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdate(entity, queryGroup));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                queryGroup.Reset();
                var queryResult = repository.Query(queryGroup);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnDecimal);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExcpetionAtSqlConnectionInlineUpdateIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                repository.InlineUpdate(entity, last.Id);
            }
        }

        #endregion

        #region InlineUpdateAsync

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, last.Id).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaDynamic()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, new { last.Id }).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaExpression()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, e => e.Id == last.Id).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaQueryField()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, new QueryField(nameof(SimpleTable.Id), last.Id)).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = repository.Query(item => item.Id == last.Id);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaQueryFields()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 10),
                new QueryField(nameof(SimpleTable.ColumnBit), true)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, fields).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                fields.ResetAll();
                var queryResult = repository.Query(fields);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnDecimal);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInlineUpdateAsyncViaQueryGroup()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 10),
                new QueryField(nameof(SimpleTable.ColumnBit), true)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = Convert.ToInt32(repository.InlineUpdateAsync(entity, queryGroup).Result.Extract());

                // Assert
                Assert.AreEqual(1, result);

                // Act
                queryGroup.Reset();
                var queryResult = repository.Query(queryGroup);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNotNull(first.ColumnDecimal);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExcpetionAtSqlConnectionInlineUpdateAsyncIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.InlineUpdateAsync(entity, last.Id).Result.Extract();
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestBaseRepositoryInsert()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestBaseRepositoryInsertAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.InsertAsync(item).Result.Extract()));

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestBaseRepositoryMerge()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult);

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithPrimaryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new Field(nameof(SimpleTable.Id)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new Field(nameof(SimpleTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, Field.From(nameof(SimpleTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, new[]
                {
                    new Field(nameof(SimpleTable.ColumnInt)),
                    new Field(nameof(SimpleTable.ColumnBit))
                });

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.Merge(queryResult, Field.From(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnBit)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestBaseRepositoryMergeAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithPrimaryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new Field(nameof(SimpleTable.Id))).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(last.Id).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual(0, queryResult.ColumnInt);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new Field(nameof(SimpleTable.ColumnInt))).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, Field.From(nameof(SimpleTable.ColumnInt))).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10).First();

                // Assert
                Assert.AreEqual(false, queryResult.ColumnBit);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, new[]
                {
                    new Field(nameof(SimpleTable.ColumnInt)),
                    new Field(nameof(SimpleTable.ColumnBit))
                }).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = repository.MergeAsync(queryResult, Field.From(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnBit))).Result.Extract();

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = repository.Query(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestBaseRepositoryQuery()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 3;

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.First(), result.Last());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(top: top, orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                AssertPropertiesEquality(tables.ElementAt(9), result.First());
                AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(new QueryField(nameof(SimpleTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(fields);

                // Assert
                Assert.AreEqual(4, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(fields, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(fields, orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(fields, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 5),
                new QueryField(nameof(SimpleTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(queryGroup);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(queryGroup, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(queryGroup, orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.StartsWith("NVAR") == true);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.StartsWith("NVAR") == false);

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.StartsWith("NVAR") != false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => !c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.Query(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestBaseRepositoryQueryAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync().Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 3;

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(top: top).Result.Extract();

                // Assert
                Assert.AreEqual(top, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(orderBy: orderBy.AsEnumerable()).Result.Extract();

                // Assert
                AssertPropertiesEquality(tables.First(), result.Last());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(top: top, orderBy: orderBy.AsEnumerable()).Result.Extract();

                // Assert
                Assert.AreEqual(result.Count(), top);
                AssertPropertiesEquality(tables.ElementAt(9), result.First());
                AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(last.Id).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(new { last.Id }).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.Id == last.Id).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(new QueryField(nameof(SimpleTable.Id), last.Id)).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(fields).Result.Extract();

                // Assert
                Assert.AreEqual(4, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(fields, top: top).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(fields, orderBy: orderBy.AsEnumerable()).Result.Extract();

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(fields, orderBy: orderBy.AsEnumerable(), top: top).Result.Extract();

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), 5),
                new QueryField(nameof(SimpleTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(queryGroup).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(queryGroup, top: top).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(queryGroup, orderBy: orderBy.AsEnumerable()).Result.Extract();

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(SimpleTable.ColumnInt), Order.Descending);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result.Extract();

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar)).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9")).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("NVAR")).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9")).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result.Extract();

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result.Extract();

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) == true).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) == false).Result.Extract();

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) != false).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => !values.Contains(c.ColumnNVarChar)).Result.Extract();

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result.Extract();

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") == true).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") == false).Result.Extract();

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") != false).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.Contains("9")).Result.Extract();

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") == true).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") == false).Result.Extract();

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") != false).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.StartsWith("NVAR")).Result.Extract();

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") == true).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") == false).Result.Extract();

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") != false).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.EndsWith("9")).Result.Extract();

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region Truncate

        [TestMethod]
        public void TestBaseRepositoryTruncate()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                repository.Truncate();

                // Act
                var result = repository.Count();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestBaseRepositoryTruncateAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var task = repository.TruncateAsync();
                task.Wait();
                task.Result.Extract();

                // Act
                var result = repository.Count();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestBaseRepositoryUpdateViaDataEntity()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                    var affectedRows = repository.Update(item);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), 10);

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnBit), true),
                new QueryField(nameof(SimpleTable.ColumnInt), 10)
            };

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnBit), true),
                new QueryField(nameof(SimpleTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
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
                var result = repository.Query(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                    var affectedRows = repository.UpdateAsync(item).Result.Extract();

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                    var affectedRows = repository.UpdateAsync(item, item.Id).Result.Extract();

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                    Assert.AreEqual(1, affectedRows.Extract());
                });

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
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
                    Assert.AreEqual(1, affectedRows.Extract());
                });

                // Act
                var result = repository.Query();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), 10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, field).Result.Extract();

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = repository.Query(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnBit), true),
                new QueryField(nameof(SimpleTable.ColumnInt), 10)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, fields).Result.Extract();

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = repository.Query(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnBit), true),
                new QueryField(nameof(SimpleTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(last, queryGroup).Result.Extract();

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = repository.Query(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestBaseRepositoryExecuteQuery()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable]");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("SELECT TOP (@Top) * FROM [dbo].[SimpleTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("[dbo].[sp_get_simple_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var simpleTableRepository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(simpleTableRepository.Insert(item)));

                using (var liteSimpleTableRepository = new SimpleTableRepository())
                {
                    // Act
                    var result = liteSimpleTableRepository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable];");

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
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable]").Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result.Extract();

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("SELECT TOP (@Top) * FROM [dbo].[SimpleTable];",
                    new { Top = 2 }).Result.Extract();

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("[dbo].[sp_get_simple_tables]",
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var simpleTableRepository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(simpleTableRepository.Insert(item)));

                using (var liteSimpleTableRepository = new SimpleTableRepository())
                {
                    // Act
                    var result = liteSimpleTableRepository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable];").Result.Extract();

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
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithNoAffectedTableRows()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteNonQuery("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;");

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteSingle()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [dbo].[SimpleTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[SimpleTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[SimpleTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new SimpleTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithNoAffectedTableRows()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteNonQueryAsync("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;").Result.Extract();

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteSingle()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = 10;").Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [dbo].[SimpleTable];").Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100;").Result.Extract();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[SimpleTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = 1;").Result.Extract();

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [dbo].[SimpleTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[SimpleTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[SimpleTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result.Extract();

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteNonQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithoutRowsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithSingleRowAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithMultipleRowsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new SimpleTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteScalar("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteScalar("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithoutRowsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result.Extract();

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithSingleRowAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1;").Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithMultipleRowsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 2 UNION ALL SELECT 1;").Result.Extract();

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1 AS Value1, 2 AS Value2;").Result.Extract();

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1;", param).Result.Extract();

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1, @Value2;", param).Result.Extract();

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result.Extract();

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalarAsync("[dbo].[sp_get_simple_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result.Extract();

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        #endregion
    }
}
