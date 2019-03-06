using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class SqlConnectionOperationsTest
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

        #region Helper

        private DateTime EpocDate => DateTime.Parse("1970-01-01 00:00:00");

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
        public void TestSqlConnectionBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQuerySecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQuerySecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryForQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryForQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryForQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<SimpleTable>(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(0), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(3), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(20);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<SimpleTable>(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 4)
                AssertPropertiesEquality(tables.ElementAt(10), result.Result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.Result.ElementAt(3));
            }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act BulkInsert
                var bulkInsertResult = connection.BulkInsert(tables);

                // Act
                var queryResult = connection.Query<SimpleTable>();

                // Assert (0, 4)
                Assert.AreEqual(tables.Count, bulkInsertResult);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act BulkInsert
                var bulkInsertResult = connection.BulkInsert(tables);

                // Act
                var queryResult = connection.Query<SimpleTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act BulkInsert
                connection.BulkInsert(tables, mappings);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesDbDataReader()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert<SimpleTable>((DbDataReader)reader);

                        // Act
                        var result = destinationConnection.Query<SimpleTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesDbDataReaderWithMappings()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert<SimpleTable>((DbDataReader)reader, mappings);

                        // Act
                        var result = destinationConnection.Query<SimpleTable>();

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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert<SimpleTable>((DbDataReader)reader, mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataReader()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(SimpleTable));

                        // Act
                        var result = destinationConnection.Query<SimpleTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataReaderWithMappings()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(SimpleTable), mappings);

                        // Act
                        var result = destinationConnection.Query<SimpleTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, result.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheMappingsAreInvalid()
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

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(SimpleTable), mappings);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert((DbDataReader)reader, "CompleteTable");
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act BulkInsert
                        destinationConnection.BulkInsert((DbDataReader)reader, "MissingTable");
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForEntities()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);

        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Act BulkInsertAsync
        //        var bulkInsertResult = connection.BulkInsertAsync(tables);

        //        // Act
        //        var queryResult = connection.QueryAsync<SimpleTable>();

        //        // Assert (0, 4)
        //        Assert.AreEqual(tables.Count, bulkInsertResult.Result);
        //        Assert.AreEqual(tables.Count, queryResult.Result.Count());
        //        tables.ToList().ForEach(t =>
        //        {
        //            AssertPropertiesEquality(t, queryResult.Result.ElementAt(tables.IndexOf(t)));
        //        });
        //    }
        //}

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForEntitiesWithMappings()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add the mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnInt)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnNVarChar)));

        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Act BulkInsertAsync
        //        var bulkInsertResult = connection.BulkInsertAsync(tables);

        //        // Act
        //        var queryResult = connection.QueryAsync<SimpleTable>();

        //        // Assert
        //        Assert.AreEqual(tables.Count, bulkInsertResult.Result);
        //        Assert.AreEqual(tables.Count, queryResult.Result.Count());
        //        tables.ToList().ForEach(t =>
        //        {
        //            AssertPropertiesEquality(t, queryResult.Result.ElementAt(tables.IndexOf(t)));
        //        });
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add invalid mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnNVarChar)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnInt)));

        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Act BulkInsertAsync
        //        var bulkInsertResult = connection.BulkInsertAsync(tables, mappings).Result;
        //    }
        //}

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReader()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                destinationConnection.BulkInsertAsync<SimpleTable>((DbDataReader)reader);

        //                // Act
        //                var queryResult = destinationConnection.QueryAsync<SimpleTable>();

        //                // Assert
        //                Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderWithMappings()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add the mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnInt)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnNVarChar)));

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                destinationConnection.BulkInsertAsync<SimpleTable>((DbDataReader)reader, mappings);

        //                // Act
        //                var queryResult = destinationConnection.QueryAsync<SimpleTable>();

        //                // Assert
        //                Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
        //            }
        //        }
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add invalid mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnNVarChar)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnInt)));

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                var bulkInsertResult = destinationConnection.BulkInsertAsync<SimpleTable>((DbDataReader)reader, mappings).Result;
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReader()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(SimpleTable));

        //                // Act
        //                var queryResult = destinationConnection.QueryAsync<SimpleTable>();

        //                // Assert
        //                Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReaderWithMappings()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add the mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnInt)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnNVarChar)));

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(SimpleTable), mappings);

        //                // Act
        //                var queryResult = destinationConnection.QueryAsync<SimpleTable>();

        //                // Assert
        //                Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
        //            }
        //        }
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);
        //    var mappings = new List<BulkInsertMapItem>();

        //    // Add invalid mappings
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnBit), nameof(SimpleTable.ColumnBit)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime), nameof(SimpleTable.ColumnDateTime)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDateTime2), nameof(SimpleTable.ColumnDateTime2)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnDecimal), nameof(SimpleTable.ColumnDecimal)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnFloat), nameof(SimpleTable.ColumnFloat)));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnNVarChar)));
        //    mappings.Add(new BulkInsertMapItem(nameof(SimpleTable.ColumnNVarChar), nameof(SimpleTable.ColumnInt)));

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(SimpleTable), mappings).Result;
        //            }
        //        }
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, "CompleteTable").Result;
        //            }
        //        }
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(InvalidOperationException))]
        //public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
        //{
        //    // Setup
        //    var tables = CreateSimpleTables(10);

        //    // Insert the records first
        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        tables.ForEach(t => connection.Insert(t));
        //    }

        //    // Open the source connection
        //    using (var sourceConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Read the data from source connection
        //        using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [SimpleTable];"))
        //        {
        //            // Open the destination connection
        //            using (var destinationConnection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //            {
        //                // Act BulkInsertAsync
        //                var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, "MissingTable").Result;
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Count

        [TestMethod]
        public void TestSqlConnectionCount()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<SimpleTable>();

                // Assert
                AssertPropertiesEquality(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<SimpleTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                AssertPropertiesEquality(7, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<SimpleTable>(field);

                // Assert
                AssertPropertiesEquality(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<SimpleTable>(fields);

                // Assert
                AssertPropertiesEquality(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<SimpleTable>(queryGroup);

                // Assert
                AssertPropertiesEquality(3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestSqlConnectionCountAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<SimpleTable>();

                // Assert
                AssertPropertiesEquality(tables.Count, result.Result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<SimpleTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                AssertPropertiesEquality(7, result.Result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<SimpleTable>(field);

                // Assert
                AssertPropertiesEquality(5, result.Result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<SimpleTable>(fields);

                // Assert
                AssertPropertiesEquality(3, result.Result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<SimpleTable>(queryGroup);

                // Assert
                AssertPropertiesEquality(3, result.Result);
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(entity)));
                });

                // Act
                var result = connection.Delete<SimpleTable>(maxId);

                // Assert
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Delete<SimpleTable>(new QueryField(nameof(SimpleTable.ColumnInt), 6));

                // Assert
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Delete<SimpleTable>(fields);

                // Assert
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Delete<SimpleTable>(queryGroup);

                // Assert
                AssertPropertiesEquality(1, result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(entity)));
                });

                // Act
                var result = connection.DeleteAsync<SimpleTable>(maxId);

                // Assert
                AssertPropertiesEquality(1, result.Result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var field = new QueryField(nameof(SimpleTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAsync<SimpleTable>(field);

                // Assert
                AssertPropertiesEquality(1, result.Result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAsync<SimpleTable>(fields);

                // Assert
                AssertPropertiesEquality(1, result.Result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var fields = new[]
            {
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(SimpleTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAsync<SimpleTable>(queryGroup);

                // Assert
                AssertPropertiesEquality(1, result.Result);
                AssertPropertiesEquality(tables.Count - 1, connection.Count<SimpleTable>());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestSqlConnectionDeleteAll()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAll<SimpleTable>();

                // Assert
                AssertPropertiesEquality(0, result);
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAllAsync<SimpleTable>();

                // Assert
                AssertPropertiesEquality(0, result.Result);
            }
        }

        #endregion

        #region InlineInsert

        [TestMethod]
        public void TestSqlConnectionInlineInsert()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineInsert<SimpleTable>(entity));

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<SimpleTable>();
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InlineInsert<SimpleTable>(entity);
            }
        }

        #endregion

        #region InlineInsertAsync

        [TestMethod]
        public void TestSqlConnectionInlineInsertAsync()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineInsertAsync<SimpleTable>(entity).Result);

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<SimpleTable>();
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineInsertAsync<SimpleTable>(entity).Result;
            }
        }

        #endregion

        #region InlineMerge

        [TestMethod]
        public void TestSqlConnectionInlineMergeWithEmptyTables()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineMerge<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>();
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
        public void TestSqlConnectionInlineMergeToExistingData()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineMerge<SimpleTable>(entity, Field.From(nameof(SimpleTable.ColumnInt))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == entity.ColumnInt);
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InlineMerge<SimpleTable>(entity);
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InlineMerge<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id)));
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InlineMerge<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id)));
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InlineMerge<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id)));
            }
        }

        #endregion

        #region InlineMergeAsync

        [TestMethod]
        public void TestSqlConnectionInlineMergeAsyncWithEmptyTables()
        {
            // Setup
            var entity = new
            {
                Id = 100,
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineMergeAsync<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id))).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>();
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
        public void TestSqlConnectionInlineMergeAsyncToExistingData()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineMergeAsync<SimpleTable>(entity, Field.From(nameof(SimpleTable.ColumnInt))).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == entity.ColumnInt);
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineMergeAsync<SimpleTable>(entity).Result;
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineMergeAsync<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id))).Result;
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineMergeAsync<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id))).Result;
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineMergeAsync<SimpleTable>(entity, Field.From(nameof(SimpleTable.Id))).Result;
            }
        }

        #endregion

        #region InlineUpdate

        [TestMethod]
        public void TestSqlConnectionInlineUpdateViaPrimaryKey()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<SimpleTable>(entity, maxId));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateViaExpression()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<SimpleTable>(entity, e => e.Id == maxId));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateViaQueryField()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<SimpleTable>(entity, new QueryField(nameof(SimpleTable.Id), maxId)));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateViaQueryFields()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<SimpleTable>(entity, fields));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(fields);
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
        public void TestSqlConnectionInlineUpdateViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<SimpleTable>(entity, queryGroup));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(queryGroup);
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
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                connection.InlineUpdate<SimpleTable>(entity, maxId);
            }
        }

        #endregion

        #region InlineUpdateAsync

        [TestMethod]
        public void TestSqlConnectionInlineUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<SimpleTable>(entity, maxId).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateAsyncViaExpression()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<SimpleTable>(entity, e => e.Id == maxId).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateAsyncViaQueryField()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<SimpleTable>(entity, new QueryField(nameof(SimpleTable.Id), maxId)).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(item => item.Id == maxId);
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
        public void TestSqlConnectionInlineUpdateAsyncViaQueryFields()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<SimpleTable>(entity, fields).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(fields);
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
        public void TestSqlConnectionInlineUpdateAsyncViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<SimpleTable>(entity, queryGroup).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SimpleTable>(queryGroup);
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
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act
                var result = connection.InlineUpdateAsync<SimpleTable>(entity, maxId).Result;
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsert()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var result = connection.Query<SimpleTable>();

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
        public void TestSqlConnectionInsertAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(table =>
                {
                    // Act (Insert)
                    var id = Convert.ToInt32(connection.InsertAsync(table).Result);

                    // Act (Query)
                    var result = connection.Query<SimpleTable>(id);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    AssertPropertiesEquality(table, result.First());
                });
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMerge()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(maxId).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult);

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(maxId).First();

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
        public void TestSqlConnectionMergeWithPrimaryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(maxId).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult, new Field(nameof(SimpleTable.Id)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(maxId).First();

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
        public void TestSqlConnectionMergeWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult, new Field(nameof(SimpleTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

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
        public void TestSqlConnectionMergeWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult, Field.From(nameof(SimpleTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

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
        public void TestSqlConnectionMergeWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult, new[]
                {
                    new Field(nameof(SimpleTable.ColumnInt)),
                    new Field(nameof(SimpleTable.ColumnBit))
                });

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.Merge(queryResult, Field.From(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnBit)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

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
        public void TestSqlConnectionMergeAsync()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(maxId).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(maxId).First();

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
        public void TestSqlConnectionMergeAsyncWithPrimaryField()
        {
            // Setup
            var tables = CreateSimpleTables(10);
            var maxId = 0;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item =>
                {
                    maxId = Math.Max(maxId, Convert.ToInt32(connection.Insert(item)));
                });

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(maxId).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult, new Field(nameof(SimpleTable.Id))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(maxId).First();

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
        public void TestSqlConnectionMergeAsyncWithNonPrimaryFieldViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult, new Field(nameof(SimpleTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

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
        public void TestSqlConnectionMergeAsyncWithNonPrimaryFieldViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult, Field.From(nameof(SimpleTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10).First();

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
        public void TestSqlConnectionMergeAsyncWithMultipleFieldsViaInstantiation()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult, new[]
                {
                    new Field(nameof(SimpleTable.ColumnInt)),
                    new Field(nameof(SimpleTable.ColumnBit))
                }).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncWithMultipleFieldsViaFromMethod()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act (Insert)
                tables.ForEach(item => connection.Insert(item));

                // Act (Query)
                var queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act (Merge)
                var mergeResult = connection.MergeAsync(queryResult, Field.From(nameof(SimpleTable.ColumnInt), nameof(SimpleTable.ColumnBit))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act (Requery)
                queryResult = connection.Query<SimpleTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Assert
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime);
                Assert.AreEqual(Helper.GetEpocDate(), queryResult.ColumnDateTime2);
                Assert.AreEqual(0, queryResult.ColumnDecimal);
                Assert.AreEqual(0, queryResult.ColumnFloat);
                Assert.AreEqual("Merged", queryResult.ColumnNVarChar);
            }
        }

        #endregion
    }
}
