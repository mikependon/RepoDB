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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = CreateSimpleTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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
                // Act Insert (** do not use bulk-insert **)
                tables.ForEach(entity => connection.Insert(entity));

                // Act Query
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

                // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act BulkInsert
                var bulkInsertResult = connection.BulkInsert(tables);

                // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

            // Switched
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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

                        // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

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

                        // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

            // Switched
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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

                        // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

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

                        // Act Query
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
            mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
            mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
            mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
            mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

            // Switched
            mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
            mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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

        //        // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

        //    using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
        //    {
        //        // Act BulkInsertAsync
        //        var bulkInsertResult = connection.BulkInsertAsync(tables);

        //        // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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

        //                // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

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

        //                // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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

        //                // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnInt"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnNVarChar"));

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

        //                // Act Query
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
        //    mappings.Add(new BulkInsertMapItem("ColumnBit", "ColumnBit"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime", "ColumnDateTime"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDateTime2", "ColumnDateTime2"));
        //    mappings.Add(new BulkInsertMapItem("ColumnDecimal", "ColumnDecimal"));
        //    mappings.Add(new BulkInsertMapItem("ColumnFloat", "ColumnFloat"));

        //    // Switched
        //    mappings.Add(new BulkInsertMapItem("ColumnInt", "ColumnNVarChar"));
        //    mappings.Add(new BulkInsertMapItem("ColumnNVarChar", "ColumnInt"));

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
    }
}
