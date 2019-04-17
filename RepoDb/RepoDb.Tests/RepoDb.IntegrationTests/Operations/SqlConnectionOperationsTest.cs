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

        private DateTime EpocDate => Helper.GetEpocDate();

        private List<IdentityTable> GetIdentityTables(int count)
        {
            var tables = new List<IdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new IdentityTable
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        private IdentityTable GetIdentityTable()
        {
            var random = new Random();
            return new IdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        private NonIdentityTable GetNonIdentityTable()
        {
            var random = new Random();
            return new NonIdentityTable
            {
                Id = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 3)
                AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
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
        public void TestSqlConnectionBatchQueryForDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryForQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryForQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    where: fields,
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
        public void TestSqlConnectionBatchQueryForQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    where: queryGroup,
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

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (0, 3)
                AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (4, 7)
                AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (5, 2)
                AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (14, 17)
                AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = GetIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: BatchQuerySecondPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (15, 12)
                AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: new { ColumnInt = 3 },
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: field,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: fields,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncForQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    where: queryGroup,
                    page: BatchQueryFirstPage,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Act
                var queryResult = connection.Query<IdentityTable>();

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
        public void TestSqlConnectionBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Act
                var queryResult = connection.Query<IdentityTable>();

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
            var tables = GetIdentityTables(10);
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.BulkInsert(tables, mappings);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesDbDataReader()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert<IdentityTable>((DbDataReader)reader);

                        // Act
                        var result = destinationConnection.Query<IdentityTable>();

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
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert<IdentityTable>((DbDataReader)reader, mappings);

                        // Act
                        var result = destinationConnection.Query<IdentityTable>();

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
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert<IdentityTable>((DbDataReader)reader, mappings);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataReader()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(IdentityTable));

                        // Act
                        var result = destinationConnection.Query<IdentityTable>();

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
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(IdentityTable), mappings);

                        // Act
                        var result = destinationConnection.Query<IdentityTable>();

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
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert((DbDataReader)reader, nameof(IdentityTable), mappings);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert((DbDataReader)reader, "CompleteTable");
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert((DbDataReader)reader, "MissingTable");
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Act
                var queryResult = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result);
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ToList().ForEach(t =>
                {
                    AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime), nameof(IdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDateTime2), nameof(IdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnDecimal), nameof(IdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnFloat), nameof(IdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(IdentityTable.ColumnNVarChar), nameof(IdentityTable.ColumnNVarChar)));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables);
                bulkInsertResult.Wait();

                // Act
                var queryResult = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult.Result);
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
            var tables = GetIdentityTables(10);
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, mappings);
                bulkInsertResult.Wait();

                // Trigger
                var result = bulkInsertResult.Result;
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader);
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = destinationConnection.QueryAsync<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderWithMappings()
        {
            // Setup
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings);
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = destinationConnection.QueryAsync<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings);
                        bulkInsertResult.Wait();

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(IdentityTable));
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = destinationConnection.QueryAsync<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReaderWithMappings()
        {
            // Setup
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(IdentityTable), mappings);
                        bulkInsertResult.Wait();

                        // Act
                        var queryResult = destinationConnection.QueryAsync<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Result.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = GetIdentityTables(10);
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, nameof(IdentityTable), mappings);
                        bulkInsertResult.Wait();

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, "CompleteTable");
                        bulkInsertResult.Wait();

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = GetIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                tables.ForEach(t => connection.Insert(t));
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync((DbDataReader)reader, "MissingTable");
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
        public void TestSqlConnectionCount()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>(field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.Count<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestSqlConnectionCountAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>(field).Result;

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.CountAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteWithoutCondition()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>(last);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(new { ColumnInt = 6 }).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(c => c.ColumnInt == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.Count<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.DeleteAsync<IdentityTable>(last).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.Count<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestSqlConnectionDeleteAll()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(entity => connection.Insert(entity));

                // Act
                var result = connection.DeleteAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(10, result);
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
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineInsert<IdentityTable>(entity));

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(result);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineInsertForIdentiyTable()
        {
            // Setup
            var entity = new
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineInsert<IdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(result);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineInsertForNonIdentityTable()
        {
            // Setup
            var entity = new
            {
                Id = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineInsert<NonIdentityTable, Guid>(entity);

                // Assert
                Assert.AreEqual(entity.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(result);
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
                connection.InlineInsert<IdentityTable>(entity);
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
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineInsertAsync<IdentityTable>(entity).Result);

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(result);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineInsertAsyncForIdentiyTable()
        {
            // Setup
            var entity = new
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineInsertAsync<IdentityTable, long>(entity).Result;

                // Assert
                Assert.IsTrue(result > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(result);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineInsertAsyncForNonIdentityTable()
        {
            // Setup
            var entity = new
            {
                Id = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InlineInsertAsync<NonIdentityTable, Guid>(entity).Result;

                // Assert
                Assert.AreEqual(entity.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(result);
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
                var result = connection.InlineInsertAsync<IdentityTable>(entity).Result;
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
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineMerge<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineMergeToExistingData()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var entity = new
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineMerge<IdentityTable>(entity, Field.From(nameof(IdentityTable.ColumnInt))));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == entity.ColumnInt);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
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
                connection.InlineMerge<IdentityTable>(entity);
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
                connection.InlineMerge<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id)));
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
                connection.InlineMerge<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id)));
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
                connection.InlineMerge<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id)));
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
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = Convert.ToInt32(connection.InlineMergeAsync<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id))).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>();
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
                Assert.AreEqual(entity.ColumnInt, first.ColumnInt);
                Assert.AreEqual(entity.ColumnDateTime2, first.ColumnDateTime2.Value);
                Assert.AreEqual(entity.ColumnNVarChar, first.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInlineMergeAsyncToExistingData()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var entity = new
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineMergeAsync<IdentityTable>(entity, Field.From(nameof(IdentityTable.ColumnInt))).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == entity.ColumnInt);
                var first = queryResult.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Assert.IsNull(first.ColumnBit);
                Assert.IsNull(first.ColumnDateTime);
                Assert.IsNull(first.ColumnDecimal);
                Assert.IsNull(first.ColumnFloat);
                Assert.IsNotNull(first.RowGuid);
                Assert.IsNotNull(first.ColumnInt);
                Assert.IsNotNull(first.ColumnDateTime2);
                Assert.IsNotNull(first.ColumnNVarChar);
                Assert.AreEqual(entity.RowGuid, first.RowGuid);
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
                var result = connection.InlineMergeAsync<IdentityTable>(entity).Result;
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
                var result = connection.InlineMergeAsync<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id))).Result;
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
                var result = connection.InlineMergeAsync<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id))).Result;
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
                var result = connection.InlineMergeAsync<IdentityTable>(entity, Field.From(nameof(IdentityTable.Id))).Result;
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, last.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
        public void TestSqlConnectionInlineUpdateViaDynamic()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, new { last.Id }));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(new { last.Id });
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, e => e.Id == last.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, new QueryField(nameof(IdentityTable.Id), last.Id)));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 10),
                new QueryField(nameof(IdentityTable.ColumnBit), true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, fields));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields);
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
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 10),
                new QueryField(nameof(IdentityTable.ColumnBit), true)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdate<IdentityTable>(entity, queryGroup));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup);
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
        public void ThrowExcpetionAtDbRepositoryInlineUpdateIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                connection.InlineUpdate<IdentityTable>(entity, last.Id);
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, last.Id).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
        public void TestSqlConnectionInlineUpdateAsyncViaDynamic()
        {
            // Setup
            var entity = new
            {
                ColumnInt = 100,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnNVarChar = Helper.GetUnicodeString()
            };
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, new { last.Id }).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(new { last.Id });
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, e => e.Id == last.Id).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, new QueryField(nameof(IdentityTable.Id), last.Id)).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.Id == last.Id);
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
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 10),
                new QueryField(nameof(IdentityTable.ColumnBit), true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, fields).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields);
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
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 10),
                new QueryField(nameof(IdentityTable.ColumnBit), true)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = Convert.ToInt32(connection.InlineUpdateAsync<IdentityTable>(entity, queryGroup).Result);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup);
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
        public void ThrowExcpetionAtDbRepositoryInlineUpdateAsyncIfTheValuesAreInvalid()
        {
            // Setup
            var entity = new
            {
                ColumnInt = "Invalid"
            };
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.InlineUpdateAsync<IdentityTable>(entity, last.Id).Result;
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsert()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = GetIdentityTable();

                // Act
                item.Id = connection.Insert<IdentityTable, long>(item);

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = GetNonIdentityTable();

                // Act
                var value = connection.Insert<NonIdentityTable, Guid>(item);

                // Act
                var result = connection.Query<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestSqlConnectionInsertAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.InsertAsync(item).Result));

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = GetIdentityTable();

                // Act
                item.Id = connection.InsertAsync<IdentityTable, long>(item).Result;

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = GetNonIdentityTable();

                // Act
                var value = connection.InsertAsync<NonIdentityTable, Guid>(item).Result;

                // Act
                var result = connection.Query<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMerge()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult);

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(last.Id).First();

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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult, new Field(nameof(IdentityTable.Id)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(last.Id).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult, new Field(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult, new[]
                {
                    new Field(nameof(IdentityTable.ColumnInt)),
                    new Field(nameof(IdentityTable.ColumnBit))
                });

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.Merge(queryResult, Field.From(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnBit)));

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(last.Id).First();

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
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(last.Id).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnInt = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult, new Field(nameof(IdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(last.Id).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult, new Field(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

                // Act
                queryResult.ColumnBit = false;
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult, new[]
                {
                    new Field(nameof(IdentityTable.ColumnInt)),
                    new Field(nameof(IdentityTable.ColumnBit))
                }).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

                // Act
                queryResult.ColumnDateTime = Helper.GetEpocDate();
                queryResult.ColumnDateTime2 = Helper.GetEpocDate();
                queryResult.ColumnDecimal = 0;
                queryResult.ColumnFloat = 0;
                queryResult.ColumnNVarChar = "Merged";

                // Act
                var mergeResult = connection.MergeAsync(queryResult, Field.From(nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnBit))).Result;

                // Assert
                Assert.AreEqual(1, mergeResult);

                // Act
                queryResult = connection.Query<IdentityTable>(item => item.ColumnInt == 10 && item.ColumnBit == true).First();

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
        public void TestSqlConnectionQuery()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(top: top);

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
        public void TestSqlConnectionQueryWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.First(), result.Last());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(top: top, orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                AssertPropertiesEquality(tables.ElementAt(9), result.First());
                AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(fields);

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
        public void TestSqlConnectionQueryViaQueryFieldsWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(fields, top: top);

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
        public void TestSqlConnectionQueryViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 5),
                new QueryField(nameof(IdentityTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(queryGroup);

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
        public void TestSqlConnectionQueryViaQueryGroupWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, top: top);

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
        public void TestSqlConnectionQueryViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable());

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContains()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == false);

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => !c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.Query<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestSqlConnectionQueryAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(top: top).Result;

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
        public void TestSqlConnectionQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                AssertPropertiesEquality(tables.First(), result.Last());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(top: top, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Assert.AreEqual(result.Count(), top);
                AssertPropertiesEquality(tables.ElementAt(9), result.First());
                AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.Id == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, top: top).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), 5),
                new QueryField(nameof(IdentityTable.ColumnInt), 6)
            };
            var queryGroup = new QueryGroup(fields, Conjunction.Or);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, top: top).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithOrderBy()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithOrderByAndTop()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                AssertPropertiesEquality(tables.ElementAt(7), result.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #region Array.Contains, String.Contains, String.StartsWith, String.EndsWith

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContains()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == false).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.ToList().ForEach(table => AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple<T1, T2>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT2()
        {
            // Setup
            var tables = GetIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT3()
        {
            // Setup
            var tables = GetIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT4()
        {
            // Setup
            var tables = GetIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT5()
        {
            // Setup
            var tables = GetIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5, T6>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT6()
        {
            // Setup
            var tables = GetIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
            }
        }

        #endregion

        #region QueryMultiple<T1, T2, T3, T4, T5, T6, T7>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleT7()
        {
            // Setup
            var tables = GetIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6,
                    where7: item => item.ColumnInt == 7);

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
                AssertPropertiesEquality(tables.ElementAt(6), result.Item7.First());
            }
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region QueryMultipleAsync<T1, T2>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT2()
        {
            // Setup
            var tables = GetIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT3()
        {
            // Setup
            var tables = GetIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT4()
        {
            // Setup
            var tables = GetIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT5()
        {
            // Setup
            var tables = GetIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5, T6>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT6()
        {
            // Setup
            var tables = GetIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
            }
        }

        #endregion

        #region QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncT7()
        {
            // Setup
            var tables = GetIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
                    where1: item => item.ColumnInt == 1,
                    where2: item => item.ColumnInt == 2,
                    where3: item => item.ColumnInt == 3,
                    where4: item => item.ColumnInt == 4,
                    where5: item => item.ColumnInt == 5,
                    where6: item => item.ColumnInt == 6,
                    where7: item => item.ColumnInt == 7).Result;

                // Assert
                AssertPropertiesEquality(tables.ElementAt(0), result.Item1.First());
                AssertPropertiesEquality(tables.ElementAt(1), result.Item2.First());
                AssertPropertiesEquality(tables.ElementAt(2), result.Item3.First());
                AssertPropertiesEquality(tables.ElementAt(3), result.Item4.First());
                AssertPropertiesEquality(tables.ElementAt(4), result.Item5.First());
                AssertPropertiesEquality(tables.ElementAt(5), result.Item6.First());
                AssertPropertiesEquality(tables.ElementAt(6), result.Item7.First());
            }
        }

        #endregion

        #endregion

        #region Truncate

        [TestMethod]
        public void TestSqlConnectionTruncate()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                connection.Truncate<IdentityTable>();

                // Act
                var result = connection.Count<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestSqlConnectionTruncateAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var task = connection.TruncateAsync<IdentityTable>();
                task.Wait();

                // Act
                var result = connection.Count<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, c => c.Id == item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, c => c.Id == item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.Query<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region ExecuteQuery (Dynamics)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamics()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryForDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryForDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync (Dynamics)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamics()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.ToList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncForDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result.FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncForDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncForDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestSqlConnectionExecuteQuery()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
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
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<LiteIdentityTable>("SELECT * FROM [dbo].[IdentityTable];");

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
        public void TestSqlConnectionExecuteQueryWithDictionaryParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryGroupAsParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldAsParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new QueryField("Id", Operation.NotEqual, 1);

                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<LiteIdentityTable>("SELECT * FROM [dbo].[IdentityTable];").Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithDictionaryParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryGroupAsParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldAsParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new QueryField("Id", Operation.NotEqual, 1);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQueryMultiple (Extract)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithoutParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP 1 * FROM [dbo].[IdentityTable];
                    SELECT TOP 2 * FROM [dbo].[IdentityTable];
                    SELECT TOP 3 * FROM [dbo].[IdentityTable];
                    SELECT TOP 4 * FROM [dbo].[IdentityTable];
                    SELECT TOP 5 * FROM [dbo].[IdentityTable];"))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [dbo].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text))
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryMultipleForExtractIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQueryMultiple("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionQueryMultipleForExtractIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQueryMultiple("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryMultipleAsync (Extract)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncMultipleForExtractWithoutParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP 1 * FROM [dbo].[IdentityTable];
                    SELECT TOP 2 * FROM [dbo].[IdentityTable];
                    SELECT TOP 3 * FROM [dbo].[IdentityTable];
                    SELECT TOP 4 * FROM [dbo].[IdentityTable];
                    SELECT TOP 5 * FROM [dbo].[IdentityTable];").Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractWithMultipleTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [dbo].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractWithMultipleArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [dbo].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [dbo].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text).Result)
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryMultipleAsyncForExtractIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryMultipleAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionQueryMultipleAsyncForExtractIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryMultipleAsync("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQueryMultiple (Scalar)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';"))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region ExecuteQueryMultipleAsync (Scalar)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';").Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region ExecuteQueryMultiple (Scalar<T>)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarTWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';"))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarTWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarTWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar<int>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region ExecuteQueryMultipleAsync (Scalar<T>)

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarTWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';").Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarTWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarTWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar<int>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region ExecuteReader

        [TestMethod]
        public void TestSqlConnectionExecuteReader()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable];"))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];", new { Top = 2 }))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteReaderAsync

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable];").Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithTopParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT TOP (@Top) * FROM [dbo].[IdentityTable];", new { Top = 2 }).Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.ToList().ForEach(item => AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedure()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => AssertPropertiesEquality(item, result.ElementAt(tables.IndexOf(item))));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader).ToList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithNoAffectedTableRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQuery("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;");

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteSingle()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQuery("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithNoAffectedTableRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQueryAsync("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;").Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteSingle()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100;").Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = 1;").Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [dbo].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [dbo].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [dbo].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithoutRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithSingleRowAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithMultipleRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithoutRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithSingleRowAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithMultipleRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT 1 AS Value1, 2 AS Value2;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT @Value1;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT @Value1, @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalarAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar<T>

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithoutRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithSingleRowAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<int>("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithMultipleRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<int>("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<int>("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<DateTime>("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<DateTime>("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalar<int>("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar<object>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar<object>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync<T>

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithoutRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithSingleRowAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<int>("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithMultipleRowsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<int>("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<int>("SELECT 1 AS Value1, 2 AS Value2;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<DateTime>("SELECT @Value1;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<DateTime>("SELECT @Value1, @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = GetIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalarAsync<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarTAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<int>("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(20000, result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<object>("SELECT * FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<object>("SELECT FROM [dbo].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

    }
}
