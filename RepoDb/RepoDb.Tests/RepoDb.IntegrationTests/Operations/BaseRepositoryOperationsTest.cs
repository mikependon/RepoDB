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
        public void TestDbRepositoryBatchQueryFirstBatchInAscendingOrder()
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
        public void TestDbRepositoryBatchQueryFirstBatchInDescendingOrder()
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
        public void TestDbRepositoryBatchQuerySecondBatchInAscendingOrder()
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
        public void TestDbRepositoryBatchQuerySecondBatchInDescendingOrder()
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
        public void TestDbRepositoryBatchQueryWithWhereForFirstBatchInAscendingOrder()
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
        public void TestDbRepositoryBatchQueryWithWhereForFirstBatchInDescendingOrder()
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
        public void TestDbRepositoryBatchQueryWithWhereForSecondBatchInAscendingOrder()
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
        public void TestDbRepositoryBatchQueryWithWhereForSecondBatchInDescendingOrder()
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
        public void TestDbRepositoryBatchQueryForQueryField()
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
        public void TestDbRepositoryBatchQueryForQueryFields()
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
        public void TestDbRepositoryBatchQueryForQueryGroup()
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
        public void TestDbRepositoryBatchQueryAsyncFirstBatchInAscendingOrder()
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
                    transaction: null);

                // Assert (0, 3)
                AssertPropertiesEquality(tables.ElementAt(0), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncFirstBatchInDescendingOrder()
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
                    transaction: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncSecondBatchInAscendingOrder()
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
                    transaction: null);

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncSecondBatchInDescendingOrder()
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
                    transaction: null);

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
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
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
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
                    transaction: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
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
                    transaction: null);

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
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
                    transaction: null);

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryField()
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
                    transaction: null);

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(3), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryFields()
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
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.Extract().ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncForQueryGroup()
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
                    transaction: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.Extract().ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.Extract().ElementAt(3));
            }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestDbRepositoryBulkInsertForEntities()
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
        public void TestDbRepositoryBulkInsertForEntitiesWithMappings()
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
        public void TestDbRepositoryBulkInsertAsyncForEntities()
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
        public void TestDbRepositoryBulkInsertAsyncForEntitiesWithMappings()
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
        public void TestDbRepositoryCount()
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
                AssertPropertiesEquality(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaExpression()
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
                AssertPropertiesEquality(7, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryField()
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
                AssertPropertiesEquality(5, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryFields()
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
                AssertPropertiesEquality(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaQueryGroup()
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
                AssertPropertiesEquality(3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbRepositoryCountAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync();

                // Assert
                AssertPropertiesEquality(tables.Count, result.Result.Extract());
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                AssertPropertiesEquality(7, result.Result.Extract());
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.CountAsync(field);

                // Assert
                AssertPropertiesEquality(5, result.Result.Extract());
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryFields()
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
                var result = repository.CountAsync(fields);

                // Assert
                AssertPropertiesEquality(3, result.Result.Extract());
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaQueryGroup()
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
                var result = repository.CountAsync(queryGroup);

                // Assert
                AssertPropertiesEquality(3, result.Result.Extract());
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestDbRepositoryDeleteViaPrimaryKey()
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
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryField()
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
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryFields()
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
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaQueryGroup()
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
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var last = tables.Last();

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(last.Id);

                // Assert
                AssertPropertiesEquality(1, result.Result.Extract());
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), 6);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.DeleteAsync(field);

                // Assert
                AssertPropertiesEquality(1, result.Result.Extract());
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryFields()
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
                var result = repository.DeleteAsync(fields);

                // Assert
                AssertPropertiesEquality(1, result.Result.Extract());
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaQueryGroup()
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
                var result = repository.DeleteAsync(queryGroup);

                // Assert
                AssertPropertiesEquality(1, result.Result.Extract());
                AssertPropertiesEquality(tables.Count - 1, repository.Count());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestDbRepositoryDeleteAll()
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
                AssertPropertiesEquality(0, result);
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var repository = new SimpleTableRepository())
            {
                // Act
                tables.ForEach(entity => repository.Insert(entity));

                // Act
                var result = repository.DeleteAllAsync();

                // Assert
                AssertPropertiesEquality(0, result.Result.Extract());
            }
        }

        #endregion

        #region InlineInsert

        [TestMethod]
        public void TestDbRepositoryInlineInsert()
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
        public void TestDbRepositoryInlineInsertAsync()
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
        public void TestDbRepositoryInlineMergeWithEmptyTables()
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
        public void TestDbRepositoryInlineMergeToExistingData()
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
        public void TestDbRepositoryInlineMergeAsyncWithEmptyTables()
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
        public void TestDbRepositoryInlineMergeAsyncToExistingData()
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
        public void TestDbRepositoryInlineUpdateViaPrimaryKey()
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
        public void TestDbRepositoryInlineUpdateViaExpression()
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
        public void TestDbRepositoryInlineUpdateViaQueryField()
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
        public void TestDbRepositoryInlineUpdateViaQueryFields()
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
        public void TestDbRepositoryInlineUpdateViaQueryGroup()
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
        public void TestDbRepositoryInlineUpdateAsyncViaPrimaryKey()
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
        public void TestDbRepositoryInlineUpdateAsyncViaExpression()
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
        public void TestDbRepositoryInlineUpdateAsyncViaQueryField()
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
        public void TestDbRepositoryInlineUpdateAsyncViaQueryFields()
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
        public void TestDbRepositoryInlineUpdateAsyncViaQueryGroup()
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
        public void TestDbRepositoryInsert()
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
        public void TestDbRepositoryInsertAsync()
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
        public void TestDbRepositoryMerge()
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
        public void TestDbRepositoryMergeWithPrimaryField()
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
        public void TestDbRepositoryMergeWithNonPrimaryFieldViaInstantiation()
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
        public void TestDbRepositoryMergeWithNonPrimaryFieldViaFromMethod()
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
        public void TestDbRepositoryMergeWithMultipleFieldsViaInstantiation()
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
        public void TestDbRepositoryMergeWithMultipleFieldsViaFromMethod()
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
        public void TestDbRepositoryMergeAsync()
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
        public void TestDbRepositoryMergeAsyncWithPrimaryField()
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
        public void TestDbRepositoryMergeAsyncWithNonPrimaryFieldViaInstantiation()
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
        public void TestDbRepositoryMergeAsyncWithNonPrimaryFieldViaFromMethod()
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
        public void TestDbRepositoryMergeAsyncWithMultipleFieldsViaInstantiation()
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
        public void TestDbRepositoryMergeAsyncWithMultipleFieldsViaFromMethod()
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
        public void TestDbRepositoryQuery()
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
        public void TestDbRepositoryQueryWithTop()
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
        public void TestDbRepositoryQueryWithOrderBy()
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
        public void TestDbRepositoryQueryWithOrderByAndTop()
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
        public void TestDbRepositoryQueryViaPrimaryKey()
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
        public void TestDbRepositoryQueryViaQueryField()
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
        public void TestDbRepositoryQueryViaQueryFields()
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
        public void TestDbRepositoryQueryViaQueryFieldsWithTop()
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
        public void TestDbRepositoryQueryViaQueryFieldsWithOrderBy()
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
        public void TestDbRepositoryQueryViaQueryFieldsWithOrderByAndTop()
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
        public void TestDbRepositoryQueryViaQueryGroup()
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
        public void TestDbRepositoryQueryViaQueryGroupWithTop()
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
        public void TestDbRepositoryQueryViaQueryGroupWithOrderBy()
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
        public void TestDbRepositoryQueryViaQueryGroupWithOrderByAndTop()
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

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbRepositoryQueryAsync()
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
        public void TestDbRepositoryQueryAsyncWithTop()
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
        public void TestDbRepositoryQueryAsyncWithOrderBy()
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
        public void TestDbRepositoryQueryAsyncWithOrderByAndTop()
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
        public void TestDbRepositoryQueryAsyncViaPrimaryKey()
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
        public void TestDbRepositoryQueryAsyncViaQueryField()
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
        public void TestDbRepositoryQueryAsyncViaQueryFields()
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
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithTop()
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
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithOrderBy()
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
        public void TestDbRepositoryQueryAsyncViaQueryFieldsWithOrderByAndTop()
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
        public void TestDbRepositoryQueryAsyncViaQueryGroup()
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
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithTop()
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
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithOrderBy()
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
        public void TestDbRepositoryQueryAsyncViaQueryGroupWithOrderByAndTop()
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

        #endregion

        #region Truncate

        [TestMethod]
        public void TestDbRepositoryTruncate()
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
        public void TestDbRepositoryTruncateAsync()
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

                // Act
                var result = repository.Count();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbRepositoryUpdateViaDataEntity()
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
        public void TestDbRepositoryUpdateViaPrimaryKey()
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
        public void TestDbRepositoryUpdateViaQueryField()
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
        public void TestDbRepositoryUpdateViaQueryFields()
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
        public void TestDbRepositoryUpdateViaQueryGroup()
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
        public void TestDbRepositoryUpdateAsyncViaDataEntity()
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
        public void TestDbRepositoryUpdateAsyncViaPrimaryKey()
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
        public void TestDbRepositoryUpdateAsyncViaQueryField()
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
        public void TestDbRepositoryUpdateAsyncViaQueryFields()
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
        public void TestDbRepositoryUpdateAsyncViaQueryGroup()
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
        public void TestDbRepositoryExecuteQuery()
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
        public void TestDbRepositoryExecuteQueryWithParameters()
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
        public void TestDbRepositoryExecuteQueryWithArrayParameters()
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
        public void TestDbRepositoryExecuteQueryWithTopParameters()
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
        public void TestDbRepositoryExecuteQueryWithStoredProcedure()
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
        public void TestDbRepositoryExecuteQueryWithStoredProcedureWithParameter()
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

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfThereAreSqlStatementProblems()
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
        public void TestDbRepositoryExecuteQueryAsync()
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
        public void TestDbRepositoryExecuteQueryAsyncWithParameters()
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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameters()
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
        public void TestDbRepositoryExecuteQueryAsyncWithTopParameters()
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
        public void TestDbRepositoryExecuteQueryAsyncWithStoredProcedure()
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
        public void TestDbRepositoryExecuteQueryAsyncWithStoredProcedureWithParameter()
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

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfThereAreSqlStatementProblems()
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
        public void TestDbRepositoryExecuteNonQueryWithNoAffectedTableRows()
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
        public void TestDbRepositoryExecuteNonQueryDeleteSingle()
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
        public void TestDbRepositoryExecuteNonQueryDeleteWithSingleParameter()
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
        public void TestDbRepositoryExecuteNonQueryDeleteWithMultipleParameters()
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
        public void TestDbRepositoryExecuteNonQueryDeleteAll()
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
        public void TestDbRepositoryExecuteNonQueryUpdateSingle()
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
        public void TestDbRepositoryExecuteNonQueryUpdateWithSigleParameter()
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
        public void TestDbRepositoryExecuteNonQueryUpdateWithMultipleParameters()
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
        public void TestDbRepositoryExecuteNonQueryUpdateAll()
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
        public void TestDbRepositoryExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
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
        public void TestDbRepositoryExecuteNonQueryWithMultipleSqlStatementsWithParameters()
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
        public void TestDbRepositoryExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
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
        public void TestDbRepositoryExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
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
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryIfThereAreSqlStatementProblems()
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
        public void TestDbRepositoryExecuteNonQueryAsyncWithNoAffectedTableRows()
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
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteSingle()
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
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteWithSingleParameter()
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
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteWithMultipleParameters()
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
        public void TestDbRepositoryExecuteNonQueryAsyncDeleteAll()
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
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateSingle()
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
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateWithSigleParameter()
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
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateWithMultipleParameters()
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
        public void TestDbRepositoryExecuteNonQueryAsyncUpdateAll()
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
        public void TestDbRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
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
        public void TestDbRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
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
        public void TestDbRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
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
        public void TestDbRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
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
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
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
        public void TestDbRepositoryExecuteScalarWithoutRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarWithMultipleRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarWithSingleRowAndWithMultipleColumnsAsResult()
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
        public void TestDbRepositoryExecuteScalarWithSingleParameterAndWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarWithMultipleParametersAndWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarWithMultipleParametersAndWithMultipleRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarByExecutingAStoredProcedureWithSingleParameter()
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
        public void TestDbRepositoryExecuteScalarByExecutingAStoredProcedureWithMultipleParameters()
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
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                repository.ExecuteScalar("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarIfThereAreSqlStatementProblems()
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
        public void TestDbRepositoryExecuteScalarAsyncWithoutRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithSingleRowAndWithMultipleColumnsAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithSingleParameterAndWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleParametersAndWithSingleRowAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
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
        public void TestDbRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithSingleParameter()
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
        public void TestDbRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithMultipleParameters()
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
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new SimpleTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM [dbo].[SimpleTable] WHERE (Id = @Id);").Result.Extract();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarAsyncIfThereAreSqlStatementProblems()
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
