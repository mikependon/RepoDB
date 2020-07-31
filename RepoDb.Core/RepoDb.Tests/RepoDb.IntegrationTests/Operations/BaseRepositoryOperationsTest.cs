using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;
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

        private class IdentityTableRepository : BaseRepository<IdentityTable, SqlConnection>
        {
            public IdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        private class LiteIdentityTableRepository : BaseRepository<LiteIdentityTable, SqlConnection>
        {
            public LiteIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        private class NonIdentityTableRepository : BaseRepository<NonIdentityTable, SqlConnection>
        {
            public NonIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        private class WithExtraFieldsIdentityTableRepository : BaseRepository<WithExtraFieldsIdentityTable, SqlConnection>
        {
            public WithExtraFieldsIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        #endregion

        #region Average

        #region Average<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #endregion

        #region AverageAll

        #region AverageAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestBaseRepositoryBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    page: 0,
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
        public void TestBaseRepositoryBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    page: 0,
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
        public void TestBaseRepositoryBatchQuerySecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    page: 1,
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
        public void TestBaseRepositoryBatchQuerySecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    page: 1,
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
        public void TestBaseRepositoryBatchQueryWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null);

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: new { ColumnInt = 3 },
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: field,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: fields,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(
                    where: queryGroup,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQuery(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQuery(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    hints: null,
                    transaction: null);

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQuery(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    hints: null,
                    transaction: null);

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithExtraFieldsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQuery(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    hints: null,
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryWithExtraFieldsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQuery(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    hints: null,
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
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
        public void TestBaseRepositoryBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
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
        public void TestBaseRepositoryBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    page: 1,
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
        public void TestBaseRepositoryBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    page: 1,
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
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    transaction: null).Result;

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: new { ColumnInt = 3 },
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: field,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: fields,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(
                    where: queryGroup,
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    hints: null,
                    transaction: null).Result;

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    hints: null,
                    transaction: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithExtraFieldsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    hints: null,
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBatchQueryAsyncWithExtraFieldsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);
            }

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var result = repository.BatchQueryAsync(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    hints: null,
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestBaseRepositoryCountWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestBaseRepositoryCountAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(field).Result;

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(fields).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestBaseRepositoryCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestBaseRepositoryCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryCountAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestBaseRepositoryDeleteViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                tables.ForEach(item =>
                {
                    var result = repository.Delete(item);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(c => c.ColumnInt == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                tables.ForEach(item =>
                {
                    var result = repository.Delete(item, hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                tables.ForEach(item =>
                {
                    var result = repository.DeleteAsync(item).Result;

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(c => c.ColumnInt == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                tables.ForEach(item =>
                {
                    var result = repository.DeleteAsync(item, SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestBaseRepositoryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll();

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAllWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAllWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAll(keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestBaseRepositoryDeleteAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAllAsyncWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, repository.CountAll());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryDeleteAllAsyncWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAllAsync(keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll());
            }
        }

        #endregion

        #endregion

        #region Exists

        #region Exists

        [TestMethod]
        public void TestBaseRepositoryExistsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestBaseRepositoryInsert()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = repository.Insert<long>(item));

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertForIdentityTable()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                var id = repository.Insert<long>(item);

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertForNonIdentityTable()
        {
            using (var repository = new NonIdentityTableRepository())
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var id = repository.Insert<Guid>(item);

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = repository.Insert<long>(item, hints: SqlServerTableHints.TabLock));

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region Insert(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryInsertWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = withExtraFieldsRepository.Insert<long>(item));

                using (var repository = new IdentityTableRepository())
                {
                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                    });
                }
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestBaseRepositoryInsertAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.InsertAsync(item).Result));

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAsyncForIdentityTable()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                var id = repository.InsertAsync<long>(item).Result;

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAsyncForNonIdentityTable()
        {
            using (var repository = new NonIdentityTableRepository())
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var id = repository.InsertAsync<Guid>(item).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.InsertAsync(item, hints: SqlServerTableHints.TabLock).Result));

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region InsertAsync(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryInsertAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                tables.ForEach(item => item.Id = withExtraFieldsRepository.InsertAsync<long>(item).Result);

                using (var repository = new IdentityTableRepository())
                {
                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                    });
                }
            }
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll<TEntity>

        [TestMethod]
        public void TestBaseRepositoryInsertAllForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var item = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.IsNotNull(item);
                    Helper.AssertPropertiesEquality(table, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables, 1);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var item = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.IsNotNull(item);
                    Helper.AssertPropertiesEquality(table, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var identityTableRepository = new NonIdentityTableRepository())
                {
                    // Act
                    var result = identityTableRepository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        var item = result.FirstOrDefault(r => r.Id == table.Id);
                        Assert.IsNotNull(item);
                        Helper.AssertPropertiesEquality(table, item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables, 1);

                using (var identityTableRepository = new NonIdentityTableRepository())
                {
                    // Act
                    var result = identityTableRepository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        var item = result.FirstOrDefault(r => r.Id == table.Id);
                        Assert.IsNotNull(item);
                        Helper.AssertPropertiesEquality(table, item);
                    });
                }
            }
        }

        #endregion

        #region InsertAllAsync<TEntity>

        [TestMethod]
        public void TestBaseRepositoryInsertAllAsyncForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAllAsync(tables).Wait();

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var item = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.IsNotNull(item);
                    Helper.AssertPropertiesEquality(table, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAllAsync(tables, 1).Wait();

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var item = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.IsNotNull(item);
                    Helper.AssertPropertiesEquality(table, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllAsyncForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAllAsync(tables).Wait();

                using (var identityTableRepository = new NonIdentityTableRepository())
                {
                    // Act
                    var result = identityTableRepository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        var item = result.FirstOrDefault(r => r.Id == table.Id);
                        Assert.IsNotNull(item);
                        Helper.AssertPropertiesEquality(table, item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAllAsync(tables, 1).Wait();

                using (var identityTableRepository = new NonIdentityTableRepository())
                {
                    // Act
                    var result = identityTableRepository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(table =>
                    {
                        var item = result.FirstOrDefault(r => r.Id == table.Id);
                        Assert.IsNotNull(item);
                        Helper.AssertPropertiesEquality(table, item);
                    });
                }
            }
        }

        #endregion

        #endregion

        #region Max

        #region Max<TEntity>

        [TestMethod]
        public void TestDbRepositoryMaxWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MaxAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMaxAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region MaxAll

        #region MaxAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryMaxAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MaxAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMaxAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region Merge

        #region Merge<TEntity>

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge<Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge<Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryMergeWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.Merge(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item, hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new WithExtraFieldsIdentityTableRepository())
            {
                // Act
                repository.Insert(item);

                // Act
                var mergeResult = repository.MergeAsync(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll());

                // Act
                var queryResult = repository.Query(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll<TEntity>

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAll(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAllAsync<TEntity>

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryMergeAllAsyncForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll());

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #endregion

        #region Min

        #region Min<TEntity>

        [TestMethod]
        public void TestDbRepositoryMinWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MinAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMinAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region MinAll

        #region MinAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryMinAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MinAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMinAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestBaseRepositoryQueryWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(whereOrPrimaryKey: null,
                    top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(fields);

                // Assert
                Assert.AreEqual(4, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(fields, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(fields, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryFieldsWithOrderByAndTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(fields, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroup()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(queryGroup);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(queryGroup, top: top);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithOrderBy()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(queryGroup, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaQueryGroupWithOrderByAndTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region Query(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryQueryWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                using (var extraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    var result = extraFieldsRepository.Query((object)null);

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.First(t => t.Id == item.Id);
                        Helper.AssertPropertiesEquality(target, item);
                    });
                }
            }
        }

        #endregion

        #region Query(Array.Contains, String.Contains, String.StartsWith, String.EndsWith)

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.StartsWith("NVAR") == true);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.StartsWith("NVAR") != false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => c.ColumnNVarChar.EndsWith("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestBaseRepositoryQueryAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(whereOrPrimaryKey: null,
                    top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.Id == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(fields).Result;

                // Assert
                Assert.AreEqual(4, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(fields, top: top).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(fields, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryFieldsWithOrderByAndTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(fields, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroup()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(queryGroup).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(queryGroup, top: top).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithOrderBy()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(queryGroup, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaQueryGroupWithOrderByAndTop()
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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region QueryAsync(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                using (var extraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    var result = extraFieldsRepository.QueryAsync((object)null).Result;

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.First(t => t.Id == item.Id);
                        Helper.AssertPropertiesEquality(target, item);
                    });
                }
            }
        }

        #endregion

        #region QueryAsync(Array.Contains, String.Contains, String.StartsWith, String.EndsWith)

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) == true).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) == false).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => values.Contains(c.ColumnNVarChar) != false).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => !values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.Contains("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") == true).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") == false).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.StartsWith("NVAR") != false).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => c.ColumnNVarChar.EndsWith("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAsyncViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(c => !c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestBaseRepositoryQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryAll(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryQueryAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var windExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    var result = windExtraFieldsRepository.QueryAll();

                    // Assert
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.First(t => t.Id == item.Id);
                        Helper.AssertPropertiesEquality(target, item);
                    });
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestBaseRepositoryQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestBaseRepositoryQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryAllAsync(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var windExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    var result = windExtraFieldsRepository.QueryAllAsync().Result;

                    // Assert
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.First(t => t.Id == item.Id);
                        Helper.AssertPropertiesEquality(target, item);
                    });
                }
            }
        }

        #endregion

        #endregion

        #region Sum

        #region Sum<TEntity>

        [TestMethod]
        public void TestDbRepositorySumWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region SumAsync<TEntity>

        [TestMethod]
        public void TestDbRepositorySumAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region SumAll

        #region SumAll<TEntity>

        [TestMethod]
        public void TestDbRepositorySumAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region SumAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositorySumAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestBaseRepositoryTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                repository.Truncate();

                // Act
                var result = repository.CountAll();

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
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var task = repository.TruncateAsync();
                task.Wait();

                // Act
                var result = repository.CountAll();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestBaseRepositoryUpdateViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.Update(item,
                        c => c.ColumnFloat == item.ColumnFloat && c.ColumnNVarChar == item.ColumnNVarChar);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.Update(item, hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region Update(With Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.Update(entity, entity.Id);

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });
                }

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.Update(entity, new { entity.Id });

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });

                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.Update(entity, c => c.Id == entity.Id);

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });

                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnBit = false;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.Update(entity, field);

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    field.Reset();
                    var result = repository.Query(field);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnFloat = last.ColumnFloat * 100;
                    last.ColumnDateTime2 = DateTime.UtcNow;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.Update(entity, fields);

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    fields.ResetAll();
                    var result = repository.Query(fields);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnFloat = last.ColumnFloat * 100;
                    last.ColumnDateTime2 = DateTime.UtcNow;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.Update(entity, queryGroup);

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    queryGroup.Reset();
                    var result = repository.Query(queryGroup);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item,
                        c => c.ColumnFloat == item.ColumnFloat && c.ColumnNVarChar == item.ColumnNVarChar).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.Query(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.Query(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

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
                var result = repository.Query(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = repository.UpdateAsync(item, hints: SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAsync(With Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, entity.Id).Result;

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });

                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, new { entity.Id }).Result;

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });

                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Act
                    tables.ForEach(item =>
                    {
                        // Set Values
                        item.ColumnBit = false;
                        item.ColumnInt = item.ColumnInt * 100;
                        item.ColumnDecimal = item.ColumnDecimal * 100;

                        // Update each
                        var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                        var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, c => c.Id == entity.Id).Result;

                        // Assert
                        Assert.AreEqual(1, affectedRows);
                    });

                    // Act
                    var result = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnBit = false;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, field).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    field.Reset();
                    var result = repository.Query(field);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnFloat = last.ColumnFloat * 100;
                    last.ColumnDateTime2 = DateTime.UtcNow;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, fields).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    fields.ResetAll();
                    var result = repository.Query(fields);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAsyncWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                using (var withExtraFieldsRepository = new WithExtraFieldsIdentityTableRepository())
                {
                    // Setup
                    var last = tables.Last();

                    // Setup
                    last.ColumnFloat = last.ColumnFloat * 100;
                    last.ColumnDateTime2 = DateTime.UtcNow;
                    last.ColumnDecimal = last.ColumnDecimal * 100;

                    // Act
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                    var affectedRows = withExtraFieldsRepository.UpdateAsync(entity, queryGroup).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);

                    // Act
                    queryGroup.Reset();
                    var result = repository.Query(queryGroup);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(last, result.First());
                }
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll<TEntity>

        [TestMethod]
        public void TestBaseRepositoryUpdateAllViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAll(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAll(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAll(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        [TestMethod]
        public void TestBaseRepositoryUpdateAllAsyncViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllAsyncViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllAsyncViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryUpdateAllAsyncViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new NonIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestBaseRepositoryExecuteQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var simpleTableRepository = new IdentityTableRepository())
            {
                // Act
                simpleTableRepository.InsertAll(tables);

                using (var liteSimpleTableRepository = new IdentityTableRepository())
                {
                    // Act
                    var result = liteSimpleTableRepository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable];");

                    // Assert
                    Assert.AreEqual(10, result.Count());
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.Where(t => t.Id == item.Id).First();
                        Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                        Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                        Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                    });
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        //[TestMethod, ExpectedException(typeof(InvalidOperationException))]
        //public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        //{
        //    using (var repository = new IdentityTableRepository())
        //    {
        //        // Setup
        //        var param = new QueryField("Id", Operation.NotEqual, 1);

        //        // Act
        //        repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
        //    }
        //}

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var simpleTableRepository = new IdentityTableRepository())
            {
                // Act
                simpleTableRepository.InsertAll(tables);

                using (var liteSimpleTableRepository = new IdentityTableRepository())
                {
                    // Act
                    var result = liteSimpleTableRepository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable];").Result;

                    // Assert
                    Assert.AreEqual(10, result.Count());
                    result.AsList().ForEach(item =>
                    {
                        var target = tables.Where(t => t.Id == item.Id).First();
                        Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                        Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                        Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                    });
                }
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteQueryAsyncWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        //{
        //    using (var repository = new IdentityTableRepository())
        //    {
        //        // Setup
        //        var param = new QueryField("Id", Operation.NotEqual, 1);

        //        // Act
        //        var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
        //    }
        //}

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithNoAffectedTableRows()
        {
            using (var repository = new IdentityTableRepository())
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
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteQuery("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithNoAffectedTableRows()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteNonQueryAsync("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;").Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;").Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;").Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteQueryAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarWithoutRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
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
            using (var repository = new IdentityTableRepository())
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
            using (var repository = new IdentityTableRepository())
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
            using (var repository = new IdentityTableRepository())
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

            using (var repository = new IdentityTableRepository())
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

            using (var repository = new IdentityTableRepository())
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

            using (var repository = new IdentityTableRepository())
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
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteScalar("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteScalar("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteScalar("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithoutRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithSingleRowAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithMultipleRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT 1 AS Value1, 2 AS Value2;").Result;

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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1;", param).Result;

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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1, @Value2;", param).Result;

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

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteScalarAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteScalar<T>

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithoutRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;");

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithSingleRowAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 1;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithMultipleRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 2 UNION ALL SELECT 1;");

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<int>("SELECT 1 AS Value1, 2 AS Value2;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1, @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalar<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param);

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteScalar<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarTIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteScalar<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarTIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.ExecuteScalar<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteScalarAsync<T>

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithoutRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT * FROM (SELECT 1 AS Column1) TMP WHERE 1 = 0;").Result;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithSingleRowAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 1;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithMultipleRowsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 2 UNION ALL SELECT 1;").Result;

                // Assert
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithSingleRowAndWithMultipleColumnsAsResult()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<int>("SELECT 1 AS Value1, 2 AS Value2;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithSingleParameterAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithMultipleParametersAndWithSingleRowAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 1
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1, @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncWithMultipleParametersAndWithMultipleRowsAsResult()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = DateTime.UtcNow.AddDays(1)
            };

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<DateTime>("SELECT @Value1 AS Value1 UNION ALL SELECT @Value2;", param).Result;

                // Assert
                Assert.AreEqual(param.Value1, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteScalarAsync<long>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Last().Id, result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExecuteScalarTAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var repository = new IdentityTableRepository())
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
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarTAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestBaseRepositoryExecuteScalarTAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new IdentityTableRepository())
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion
    }
}
