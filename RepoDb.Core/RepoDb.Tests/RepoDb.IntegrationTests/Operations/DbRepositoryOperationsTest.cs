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

        #region Average

        #region Average<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region Average(TableName)

        [TestMethod]
        public void TestDbRepositoryAverageViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAll(TableName)

        [TestMethod]
        public void TestDbRepositoryAverageViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryAverageAllTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllTableNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery<TEntity>

        [TestMethod]
        public void TestDbRepositoryBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
                    transaction: null);

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
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
        public void TestDbRepositoryBatchQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
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
        public void TestDbRepositoryBatchQueryViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
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
        public void TestDbRepositoryBatchQueryViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<IdentityTable>(
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

        #region BatchQuery<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryBatchQueryWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryWithExtraFieldsViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryWithExtraFieldsViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery<WithExtraFieldsIdentityTable>(
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

        #region BatchQueryAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    hints: null,
                    transaction: null).Result;

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    hints: null,
                    transaction: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
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
        public void TestDbRepositoryBatchQueryAsyncViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
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
        public void TestDbRepositoryBatchQueryAsyncViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<IdentityTable>(
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

        #region BatchQueryAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryAsyncWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryAsyncWithExtraFieldsViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryBatchQueryAsyncWithExtraFieldsViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync<WithExtraFieldsIdentityTable>(
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

        #region BatchQuery(TableName)

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null);

                // Assert (0, 3)
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null);

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null);

                // Assert (4, 7)
                Helper.AssertMembersEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null);

                // Assert (5, 2)
                Helper.AssertMembersEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThanOrEqual, 1),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 10)
                    },
                    transaction: null);

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null);

                // Assert (14, 17)
                Helper.AssertMembersEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null);

                // Assert (15, 12)
                Helper.AssertMembersEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    transaction: null);

                // Assert (2)
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    transaction: null);

                // Assert (3, 6)
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    transaction: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (0, 3)
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (4, 7)
                Helper.AssertMembersEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null).Result;

                // Assert (5, 2)
                Helper.AssertMembersEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThanOrEqual, 1),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 10)
                    },
                    transaction: null).Result;

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null).Result;

                // Assert (14, 17)
                Helper.AssertMembersEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    transaction: null).Result;

                // Assert (15, 12)
                Helper.AssertMembersEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    transaction: null).Result;

                // Assert (2)
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    transaction: null).Result;

                // Assert (3, 6)
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQueryAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    transaction: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #endregion

        #region Count

        #region Count<TEntity>

        [TestMethod]
        public void TestDbRepositoryCountWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Count<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryCountAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region Count(TableName)

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameAsyncViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountViaTableNameAsyncViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAll(TableName)

        [TestMethod]
        public void TestDbRepositoryCountViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryCountAllTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCountAllTableNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #endregion

        #region Delete

        #region Delete<TEntity>

        [TestMethod]
        public void TestDbRepositoryDeleteViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete<IdentityTable>(new { ColumnInt = 6 });

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete<IdentityTable>(c => c.Id == last.Id);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete<IdentityTable>((object)null);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync<IdentityTable>(new { ColumnInt = 6 }).Result;

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                tables.ForEach(item =>
                {
                    var result = repository.DeleteAsync(item, hints: SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region Delete(TableName)

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteViaTableNameWithoutConditionWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 }).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAsyncViaTableNameWithoutConditionWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll<IdentityTable>(hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAll<IdentityTable>(keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync<IdentityTable>(hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAllAsync<IdentityTable>(keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAll(TableName)

        [TestMethod]
        public void TestDbRepositoryDeleteAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll(ClassMappedNameCache.Get<IdentityTable>(), hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllViaTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Select(e => (object)e.Id));

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<NonIdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllViaTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll<NonIdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>(), hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncViaTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Select(e => (object)e.Id)).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, repository.CountAll<NonIdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbRepositoryDeleteAllAsyncViaTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = repository.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, repository.CountAll<NonIdentityTable>());
            }
        }

        #endregion

        #endregion

        #region Exists

        #region Exists<TEntity>

        [TestMethod]
        public void TestDbRepositoryExistsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>(field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>(fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<IdentityTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryExistsAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsAsyncViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsAsyncViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Exists(TableName)

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameAsyncViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExistsViaTableNameAsyncViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion

        #region Insert

        #region Insert<TEntity>

        [TestMethod]
        public void TestDbRepositoryInsert()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.Insert<IdentityTable, long>(item));

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
                var id = repository.Insert<IdentityTable, long>(item);

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

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
                var id = repository.Insert<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.Insert<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock));

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

        #endregion

        #region Insert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryInsertWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.Insert<WithExtraFieldsIdentityTable, long>(item));

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

        #endregion

        #region InsertAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryInsertAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.InsertAsync<IdentityTable, long>(item).Result);

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
                var id = repository.InsertAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

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
                var id = repository.InsertAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.InsertAsync<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock).Result);

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

        #endregion

        #region InsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryInsertAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = repository.InsertAsync<WithExtraFieldsIdentityTable, long>(item).Result);

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

        #endregion

        #region Insert(TableName)

        [TestMethod]
        public void TestDbRepositoryInsertViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = repository.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item);
                });

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
        public void TestDbRepositoryInsertViaTableNameForIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = repository.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    item);

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertViaTableNameForNonIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = repository.Insert<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var item = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = repository.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(1, repository.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = repository.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item, hints: SqlServerTableHints.TabLock);
                });

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

        #endregion

        #region InsertAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryInsertAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = repository.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Result;
                });

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
        public void TestDbRepositoryInsertAsyncViaTableNameForIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = repository.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    item).Result;

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncViaTableNameForNonIdentityTable()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = repository.InsertAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item).Result;

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var item = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = repository.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Result;

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(1, repository.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = repository.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item, hints: SqlServerTableHints.TabLock).Result;
                });

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

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryInsertAllForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll<IdentityTable>();

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
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables, 1);

                // Act
                var result = repository.QueryAll<IdentityTable>();

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
        public void TestDbRepositoryInsertAllForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

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
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables, 1);

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

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

        #endregion

        #region InsertAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryInsertAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<WithExtraFieldsIdentityTable>(tables);

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
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<WithExtraFieldsIdentityTable>(tables, 1);

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

        #endregion

        #region InsertAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var value = repository.InsertAllAsync(tables).Result;

                // Act
                var result = repository.QueryAll<IdentityTable>();

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
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var value = repository.InsertAllAsync(tables, 1).Result;

                // Act
                var result = repository.QueryAll<IdentityTable>();

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
        public void TestDbRepositoryInsertAllAsyncForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(tables).Wait();

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

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
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(tables, 1).Wait();

                // Act
                var result = repository.QueryAll<NonIdentityTable>();

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

        #endregion

        #region InsertAllAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync<WithExtraFieldsIdentityTable>(tables).Wait();

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
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync<WithExtraFieldsIdentityTable>(tables, 1).Wait();

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

        #endregion

        #region InsertAll(TableName)

        [TestMethod]
        public void TestDbRepositoryInsertAllForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables, 1);

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2);

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, 1, fields: tables.Item2);

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1);

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2);

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllWithSizePerBatchEqualsToOneForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, 1, fields: tables.Item2);

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = repository.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Length, insertAllResult);
                Assert.AreEqual(tables.Length, repository.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.RowGuid == item.RowGuid)));
            }
        }

        #endregion

        #region InsertAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables, 1).Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2).Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, 1, fields: tables.Item2).Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Wait();

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1).Wait();

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2).Wait();

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, 1, fields: tables.Item2).Wait();

                // Act
                var result = repository.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryInsertAllAsyncForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = repository.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Length, insertAllResult);
                Assert.AreEqual(tables.Length, repository.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.RowGuid == item.RowGuid)));
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Max(TableName)

        [TestMethod]
        public void TestDbRepositoryMaxViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MaxAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MaxAll(TableName)

        [TestMethod]
        public void TestDbRepositoryMaxViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MaxAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMaxAllTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllTableNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
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
        public void TestDbRepositoryMergeForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<IdentityTable>(item, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryMergeWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = repository.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = repository.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = repository.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<IdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<IdentityTable>(item, hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = repository.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = repository.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = repository.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge(TableName)

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeViaTableNameForNonIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.Merge(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)item, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnDbRepositoryMergeViaTableNameIfThereIsNoPrimaryKey()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Merge(ClassMappedNameCache.Get<IdentityTable>(), (object)item);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = repository.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAsyncViaTableNameForNonIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = repository.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)item, hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryMergeAsyncViaTableNameIfThereIsNoPrimaryKey()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.MergeAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)item).Wait();
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll(TableName)

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Length, mergeAllResult);
                Assert.AreEqual(tables.Length, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnDbRepositoryMergeAllIfThereIsNoPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.MergeAll(ClassMappedNameCache.Get<IdentityTable>(), tables);
            }
        }

        #endregion

        #region MergeAll(TableName)(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = repository.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Length, mergeAllResult);
                Assert.AreEqual(tables.Length, repository.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = repository.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryMergeAllAsyncIfThereIsNoPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.MergeAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();
            }
        }

        #endregion

        #region MergeAllAsync(TableName)(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<IdentityTable>());

                // Act
                var queryResult = repository.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMergeAllAsyncViaTableNameForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = repository.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, repository.CountAll<NonIdentityTable>());

                // Act
                var queryResult = repository.QueryAll<NonIdentityTable>();

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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Min(TableName)

        [TestMethod]
        public void TestDbRepositoryMinViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MinAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MinAll(TableName)

        [TestMethod]
        public void TestDbRepositoryMinViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region MinAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryMinAllTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllTableNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query<TEntity>

        [TestMethod]
        public void TestDbRepositoryQueryWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(whereOrPrimaryKey: null,
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
        public void TestDbRepositoryQueryWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(fields);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(fields, top: top);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(queryGroup);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(queryGroup, top: top);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region Query<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryQueryWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<WithExtraFieldsIdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region Query<TEntity>(Array.Contains, String.Contains, String.StartsWith, String.EndsWith)

        [TestMethod]
        public void TestDbRepositoryQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR"));

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #region QueryAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
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
        public void TestDbRepositoryQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(last.Id).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(new { last.Id }).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.Id == last.Id).Result;

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(fields, top: top).Result;

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup, top: top).Result;

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region QueryAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryQueryAsyncWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<WithExtraFieldsIdentityTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region QueryAsync<TEntity>(Array.Contains, String.Contains, String.StartsWith, String.EndsWith)

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #region Query(TableName)

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameWithoutConditionAndWithFewFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    Field.From(new[] { "Id", "RowGuid", "ColumnFloat" }));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
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
        public void TestDbRepositoryQueryViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

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
        public void TestDbRepositoryQueryViaTableNameViaQueryFieldsWithTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    top: top);

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
        public void TestDbRepositoryQueryViaTableNameViaQueryFieldsWithOrderBy()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaQueryFieldsWithOrderByAndTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

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
        public void TestDbRepositoryQueryViaTableNameViaQueryGroupWithTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    top: top);

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
        public void TestDbRepositoryQueryViaTableNameViaQueryGroupWithOrderBy()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryViaTableNameViaQueryGroupWithOrderByAndTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable(),
                    top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnDbRepositoryQueryViaTableNameViaPrimaryKeyIfThePrimaryKeyIsNotDefinedFromTheDatabase()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    1);
            }
        }

        #endregion

        #region QueryAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameWithoutConditionAndWithFewFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    Field.From(new[] { "Id", "RowGuid", "ColumnFloat" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
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
        public void TestDbRepositoryQueryAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

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
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryFieldsWithTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    top: top).Result;

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
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryFieldsWithOrderBy()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryFieldsWithOrderByAndTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

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
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryGroupWithTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    top: top).Result;

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
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryGroupWithOrderBy()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncViaTableNameViaQueryGroupWithOrderByAndTop()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable(),
                    top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryAsyncViaTableNameViaPrimaryKeyIfThePrimaryKeyIsNotDefinedFromTheDatabase()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    1).Wait();
            }
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
        public void TestDbRepositoryQueryAllWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
        public void TestDbRepositoryQueryAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll<IdentityTable>(orderBy: orderBy,
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

        #region QueryAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryQueryAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll<WithExtraFieldsIdentityTable>();

                // Assert
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
        public void TestDbRepositoryQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
        public void TestDbRepositoryQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(orderBy: orderBy,
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

        #region QueryAllAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync<WithExtraFieldsIdentityTable>().Result;

                // Assert
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region QueryAll(TableName)

        [TestMethod]
        public void TestDbRepositoryQueryAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        #endregion

        #region QueryAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryAllAsyncViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple<TEntity>

        #region QueryMultiple<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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

        #region QueryMultiple<TEntity>(With Extra Fields)

        #region QueryMultiple<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleWithExtraFieldsT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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

        #region QueryMultipleAsync<TEntity>

        #region QueryMultipleAsync<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

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

        #region QueryMultipleAsync<TEntity>(With Extra Fields)

        #region QueryMultipleAsync<T1, T2>

        [TestMethod]
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestDbRepositoryQueryMultipleAsyncWithExtraFieldsT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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

        #endregion

        #region Sum

        #region Sum<TEntity>

        [TestMethod]
        public void TestDbRepositorySumWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Sum(TableName)

        [TestMethod]
        public void TestDbRepositorySumViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region SumAsync(TableName)

        [TestMethod]
        public void TestDbRepositorySumAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAsyncViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll<IdentityTable>(e => e.ColumnInt,
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region SumAll(TableName)

        [TestMethod]
        public void TestDbRepositorySumViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region SumAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositorySumAllTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllTableNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate<TEntity>

        [TestMethod]
        public void TestDbRepositoryTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                repository.Truncate<IdentityTable>();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryTruncateAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

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

        #region Truncate(TableName)

        [TestMethod]
        public void TestDbRepositoryTruncateViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                repository.Truncate(ClassMappedNameCache.Get<IdentityTable>());

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryTruncateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var task = repository.TruncateAsync(ClassMappedNameCache.Get<IdentityTable>());
                task.Wait();

                // Act
                var result = repository.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #endregion

        #region Update

        #region Update<TEntity>

        [TestMethod]
        public void TestDbRepositoryUpdateViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var affectedRows = repository.Update<IdentityTable>(item,
                        c => c.ColumnInt == item.ColumnInt && c.ColumnNVarChar == item.ColumnNVarChar);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region Update<TEntity>(With Extra Fields)

        [TestMethod]
        public void TestDbRepositoryUpdateWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.Update(entity, entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.Update(entity, new { entity.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.Update(entity, c => c.Id == entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.Update(entity, field);

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
        public void TestDbRepositoryUpdateWithExtraFieldViaQueryFields()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.Update(entity, fields);

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
        public void TestDbRepositoryUpdateWithExtraFieldViaQueryGroup()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.Update(entity, queryGroup);

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

        #region UpdateAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                        c => c.ColumnInt == item.ColumnInt && c.ColumnNVarChar == item.ColumnNVarChar).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                var result = repository.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAsync<TEntity>(With Extra Fields)

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.UpdateAsync(entity, entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.UpdateAsync(entity, new { entity.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = repository.UpdateAsync(entity, c => c.Id == entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.UpdateAsync(entity, field).Result;

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
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaQueryFields()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.UpdateAsync(entity, fields).Result;

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
        public void TestDbRepositoryUpdateAsyncWithExtraFieldViaQueryGroup()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = repository.UpdateAsync(entity, queryGroup).Result;

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

        #region Update(TableName)

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var affectedRows = repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item,
                    item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var affectedRows = repository.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    item,
                    new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field);

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
        public void TestDbRepositoryUpdateViaTableNameViaQueryFields()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields);

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
        public void TestDbRepositoryUpdateViaTableNameViaQueryGroup()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup);

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

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data,
                        hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnDbRepositoryUpdateViaTableNameIfThePrimaryKeyIsNotFound()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnDbRepositoryUpdateViaTableNameIfTheFieldsAreNotFound()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                repository.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        #endregion

        #region UpdateAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item,
                    item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                    var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    item,
                    new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = repository.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field).Result;

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
        public void TestDbRepositoryUpdateViaTableNameAsyncViaQueryFields()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields).Result;

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
        public void TestDbRepositoryUpdateViaTableNameAsyncViaQueryGroup()
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
                repository.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup).Result;

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

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = repository.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data,
                        hints: SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryUpdateAsyncViaTableNameIfThePrimaryKeyIsNotFound()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDbRepositoryUpdateAsyncViaTableNameIfTheFieldsAreNotFound()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                repository.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll<TEntity>

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAll(TableName)

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaTableNameWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaTableNameViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllViaDataEntitiesViaTableNameViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion

        #region UpdateAllAsync(TableName)

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), items).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaTableNameWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestDbRepositoryUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var items = tables.Select(item => new
                {
                    // Set Values
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Update each
                var affectedRows = repository.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];");

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

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWhereTheDataReaderColumnsAreLessThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT Id, ColumnBit, ColumnDateTime, ColumnInt FROM [sc].[IdentityTable];");

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
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
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                repository.InsertAll(tables);

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;");

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable];");

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;");

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
        public void TestDbRepositoryExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
        public void TestDbRepositoryExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;").Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable];").Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
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
                repository.InsertAll(tables);

                // Act
                var result = repository.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;").Result;

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
        public void TestDbRepositoryExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
        public void TestDbRepositoryExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
                repository.ExecuteScalar("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
                var result = repository.ExecuteScalarAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
                repository.ExecuteScalar<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                repository.ExecuteScalar<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
                var result = repository.ExecuteScalarAsync<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestDbRepositoryExecuteScalarTAsyncIfThereAreSqlStatementProblems()
        {
            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = repository.ExecuteScalarAsync<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion
    }
}
