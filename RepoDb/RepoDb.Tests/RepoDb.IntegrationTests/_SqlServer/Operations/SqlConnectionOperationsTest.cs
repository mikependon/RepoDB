using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
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

        #region Average

        #region Average<TEntity>

        [TestMethod]
        public void TestDbRepositoryAverageWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAllAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 3)
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQuerySecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (4, 7)
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQuerySecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (5, 2)
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQuery<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithExtraFieldsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryWithExtraFieldsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (0, 3)
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (4, 7)
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (5, 2)
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt >= 1 && item.ColumnInt <= 10,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (14, 17)
                Helper.AssertPropertiesEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: item => item.ColumnInt > 10 && item.ColumnInt <= 20,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (15, 12)
                Helper.AssertPropertiesEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (2)
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithExtraFieldsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncWithExtraFieldsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertPropertiesEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQuery(TableName)

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (0, 3)
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (4, 7)
                Helper.AssertMembersEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (5, 2)
                Helper.AssertMembersEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThanOrEqual, 1),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 10)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (14, 17)
                Helper.AssertMembersEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (15, 12)
                Helper.AssertMembersEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (3, 6)
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #region BatchQueryAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (0, 3)
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (4, 7)
                Helper.AssertMembersEquality(tables.ElementAt(4), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: (object)null,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (5, 2)
                Helper.AssertMembersEquality(tables.ElementAt(5), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThanOrEqual, 1),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 10)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (9, 6)
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (14, 17)
                Helper.AssertMembersEquality(tables.ElementAt(14), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(17), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 1,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Descending }),
                    where: new[]
                    {
                        new QueryField("ColumnInt", Operation.GreaterThan, 10),
                        new QueryField("ColumnInt", Operation.LessThanOrEqual, 20)
                    },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (15, 12)
                Helper.AssertMembersEquality(tables.ElementAt(15), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(12), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (2)
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(0));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: field,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (3, 6)
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: fields,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 10),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 20)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: queryGroup,
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (10, 13)
                Helper.AssertMembersEquality(tables.ElementAt(10), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(13), result.ElementAt(3));
            }
        }

        #endregion

        #endregion

        #region BulkInsert

        #region BulkInsert<TEntity>

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
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
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsert<IdentityTable>((DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationConnection.QueryAll<IdentityTable>();

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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsert<IdentityTable>((DbDataReader)reader, mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationConnection.QueryAll<IdentityTable>();

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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
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
        public void TestSqlConnectionBulkInsertForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert<IdentityTable>(table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesDataTableWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert<IdentityTable>(table, DataRowState.Unchanged, mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForEntitiesDataTableIfTheMappingsAreInvalid()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkInsert<IdentityTable>(table, DataRowState.Unchanged, mappings);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);
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

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionBulkInsertForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationConnection.QueryAll<IdentityTable>();

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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader, mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var result = destinationConnection.QueryAll<IdentityTable>();

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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader, mappings);

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert("InvalidTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataReaderIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        destinationConnection.BulkInsert("MissingTable", (DbDataReader)reader);
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertForTableNameDbDataTableWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), table, DataRowState.Unchanged, mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var result = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, result.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataTableIfTheMappingsAreInvalid()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsert(ClassMappedNameCache.Get<IdentityTable>(), table, DataRowState.Unchanged, mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkInsert("InvalidTable", table, DataRowState.Unchanged);
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            destinationConnection.BulkInsert("MissingTable", table, DataRowState.Unchanged);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesWithMappings()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables, mappings);

                // Trigger
                var result = bulkInsertResult.Result;
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDbDataReaderWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<IdentityTable>();

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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>((DbDataReader)reader, mappings);

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>(table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesDataTableWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>(table, DataRowState.Unchanged, mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync<IdentityTable>(table, DataRowState.Unchanged, mappings);

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
        public void TestSqlConnectionBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);
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
                var bulkInsertResult = connection.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionBulkInsertAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var bulkInsertResult = connection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDbDataReaderWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader, mappings).Result;

                        // Assert
                        Assert.AreEqual(tables.Count, bulkInsertResult);

                        // Act
                        var queryResult = destinationConnection.QueryAll<IdentityTable>();

                        // Assert
                        Assert.AreEqual(tables.Count * 2, queryResult.Count());
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDbDataReaderIfTheMappingsAreInvalid()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), (DbDataReader)reader, mappings);

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
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync("InvalidTable", (DbDataReader)reader);

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
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Open the destination connection
                    using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                    {
                        // Act
                        var bulkInsertResult = destinationConnection.BulkInsertAsync("MissingTable", (DbDataReader)reader);

                        // Trigger
                        var result = bulkInsertResult.Result;
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionBulkInsertAsyncForTableNameDataTableWithMappings()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), table, DataRowState.Unchanged, mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkInsertResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<IdentityTable>();

                            // Assert
                            Assert.AreEqual(tables.Count * 2, queryResult.Count());
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync(ClassMappedNameCache.Get<IdentityTable>(), table, DataRowState.Unchanged, mappings);

                            // Trigger
                            var result = bulkInsertResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync("InvalidTable", table);

                            // Trigger
                            var result = bulkInsertResult.Result;
                        }
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            // Insert the records first
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new SqlConnection(Database.ConnectionStringForRepoDb))
                        {
                            // Act
                            var bulkInsertResult = destinationConnection.BulkInsertAsync("MissingTable", table, DataRowState.Unchanged);

                            // Trigger
                            var result = bulkInsertResult.Result;
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Count

        #region Count<TEntity>

        [TestMethod]
        public void TestSqlConnectionCountWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionCountAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region Count(TableName)

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAll(TableName)

        [TestMethod]
        public void TestSqlConnectionCountViaAllTaleName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllViaTaleNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionCountAllTaleNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllTaleNameAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete(item);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.DeleteAsync(item).Result;

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(new { ColumnInt = 6 }).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(c => c.ColumnInt == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region Delete(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 }).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAll(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaTableNameAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #endregion

        #region Exists

        #region Exists<TEntity>

        [TestMethod]
        public void TestSqlConnectionExistsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExistsAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Exists(TableName)

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionInsert()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = connection.Insert<IdentityTable, long>(item));

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                var id = connection.Insert<IdentityTable, long>(item);

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var id = connection.Insert<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region Insert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = connection.Insert<WithExtraFieldsIdentityTable, long>(item));

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = connection.InsertAsync<IdentityTable, long>(item).Result);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                var id = connection.Insert<IdentityTable, long>(item);

                // Assert
                Assert.IsTrue(0 < id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var id = connection.InsertAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        #endregion

        #region InsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = connection.InsertAsync<WithExtraFieldsIdentityTable, long>(item).Result);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    item);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = connection.Insert<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var item = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        #endregion

        #region InsertAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Result;
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                item.Id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    item).Result;

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                var value = connection.InsertAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item).Result;

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, value);
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var item = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Result;

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables, 1);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

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
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables, 1);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

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
        public void TestSqlConnectionInsertAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneAndWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables, 1);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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

        #region InsertAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables).Wait();

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables, 1).Wait();

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllAsyncForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables).Wait();

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

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
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables, 1).Wait();

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

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
        public void TestSqlConnectionInsertAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables).Wait();

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(tables, 1).Wait();

                // Act
                var result = connection.QueryAll<IdentityTable>();

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

        #region InsertAll(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables, 1);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, fields: tables.Item2);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, 1, fields: tables.Item2);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1);

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, fields: tables.Item2);

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, 1, fields: tables.Item2);

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Length, insertAllResult);
                Assert.AreEqual(tables.Length, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.RowGuid == item.RowGuid)));
            }
        }

        #endregion

        #region InsertAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables, 1).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables.Item1, 1, fields: tables.Item2).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Wait();

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1).Wait();

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, Constant.DefaultBatchOperationSize, tables.Item2).Wait();

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables.Item1, 1, fields: tables.Item2).Wait();

                // Act
                var result = connection.CountAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Length, insertAllResult);
                Assert.AreEqual(tables.Length, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionMergeForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeViaTableNameIfThereIsNoPrimaryKey()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Merge(ClassMappedNameCache.Get<IdentityTable>(), (object)item);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAsyncViaTableNameIfThereIsNoPrimaryKey()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)item).Wait();
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<NonIdentityTable>(tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll<TEntity>(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Length, mergeAllResult);
                Assert.AreEqual(tables.Length, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeAllIfThereIsNoPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAll(ClassMappedNameCache.Get<IdentityTable>(), tables);
            }
        }

        #endregion

        #region MergeAll(TableName)(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        #endregion

        #region MergeAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new[]
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Length, mergeAllResult);
                Assert.AreEqual(tables.Length, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>());

                // Assert
                tables.ToList().ForEach(item => Helper.AssertMembersEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAllAsyncIfThereIsNoPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();
            }
        }

        #endregion

        #region MergeAllAsync(TableName)(SingleBatch, ModularBatch)

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(19);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableViaSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(item => Helper.AssertPropertiesEquality(item,
                    queryResult.First(data => data.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableViaModularBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(99);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositoryMinAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>((object)null);

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
        public void TestSqlConnectionQueryWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(whereOrPrimaryKey: null,
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
        public void TestSqlConnectionQueryWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(fields);

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
        public void TestSqlConnectionQueryViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(fields, top: top);

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
        public void TestSqlConnectionQueryViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryFieldsWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(queryGroup);

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
        public void TestSqlConnectionQueryViaQueryGroupWithTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, top: top);

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
        public void TestSqlConnectionQueryViaQueryGroupWithOrderBy()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaQueryGroupWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region Query<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<WithExtraFieldsIdentityTable>((object)null);

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
        public void TestSqlConnectionQueryViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4"));

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false);

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => !values.Contains(c.ColumnNVarChar));

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR"));

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8"));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => !c.ColumnNVarChar.Contains("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false);

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9"));

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #region QueryAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>((object)null).Result;

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
        public void TestSqlConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
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
        public void TestSqlConnectionQueryAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(whereOrPrimaryKey: null,
                    top: top,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Assert.AreEqual(result.Count(), top);
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.Id == last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, top: top).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryFieldsWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(fields, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, top: top).Result;

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
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithOrderBy()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaQueryGroupWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(queryGroup, orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        #endregion

        #region QueryAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryAsyncWithExtraFieldsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<WithExtraFieldsIdentityTable>((object)null).Result;

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
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));

            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringContains()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.Contains("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAndStringEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) || c.ColumnNVarChar.EndsWith("4")).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == true).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) == false).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => values.Contains(c.ColumnNVarChar) != false).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithArrayContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new[] { "NVARCHAR1", "NVARCHAR2" };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !values.Contains(c.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(8, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAndStartsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.StartsWith("NVAR")).Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAndEndsWith()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") || c.ColumnNVarChar.EndsWith("8")).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.Contains("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringContainsAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.Contains("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsBooleanTrue()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") == true).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.StartsWith("NVAR") != false).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringStartsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == true).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsBooleanFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") == false).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsBooleanNotEqualsToFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => c.ColumnNVarChar.EndsWith("9") != false).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.First(t => t.Id == result.First().Id), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaExpressionWithStringEndsWithAsUnaryFalse()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync<IdentityTable>(c => !c.ColumnNVarChar.EndsWith("9")).Result;

                // Assert
                Assert.AreEqual(9, result.Count());
                result.AsList().ForEach(table => Helper.AssertPropertiesEquality(tables.First(t => t.Id == table.Id), table));
            }
        }

        #endregion

        #region Query(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameWithoutConditionAndWithFewFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    last.Id);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    new { last.Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.Id), last.Id));

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaQueryFieldsWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable(), top: top);

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameViaQueryGroupWithTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryViaTableNameViaQueryGroupWithOrderBy()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable());

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTableNameViaQueryGroupWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void ThrowExceptionOnSqlConnectionQueryViaTableNameViaPrimaryKeyIfThePrimaryKeyIsNotDefinedFromTheDatabase()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    1);
            }
        }

        #endregion

        #region QueryAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameWithoutConditionAndWithFewFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    whereOrPrimaryKey: null,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.First(), result.Last());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameWithOrderByAndTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 3;
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    last.Id).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { last.Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.Id), last.Id)).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryFieldsWithTop()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var top = 2;
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryFieldsWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var orderBy = new OrderField(nameof(IdentityTable.ColumnInt), Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryFieldsWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields,
                    orderBy: orderBy.AsEnumerable(), top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryGroup()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryGroupWithTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryGroupWithOrderBy()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable()).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(4), result.Last());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncViaTableNameViaQueryGroupWithOrderByAndTop()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup,
                    orderBy: orderBy.AsEnumerable(),
                    top: top).Result;

                // Assert
                Assert.AreEqual(top, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.First());
                Helper.AssertPropertiesEquality(tables.ElementAt(5), result.Last());
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnQueryAsyncViaTableNameViaPrimaryKeyIfThePrimaryKeyIsNotDefinedFromTheDatabase()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    1).Wait();
            }
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>();

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
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy,
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
        public void TestSqlConnectionQueryAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<WithExtraFieldsIdentityTable>();

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
        public void TestSqlConnectionQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>().Result;

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
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy,
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
        public void TestSqlConnectionQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<WithExtraFieldsIdentityTable>().Result;

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
        public void TestSqlConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

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
        public void TestSqlConnectionQueryAllViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAllViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionQueryMultipleT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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

        #region QueryMultiple<T>(With Extra Fields)

        #region QueryMultiple<T1, T2>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleWithExtraFieldsT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultiple<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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

        #region QueryMultipleAsync<T>(With Extra Fields)

        #region QueryMultipleAsync<T1, T2>

        [TestMethod]
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT2()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT3()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT4()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(4);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT5()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT6()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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
        public void TestSqlConnectionQueryMultipleAsyncWithExtraFieldsT7()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(7);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryMultipleAsync<WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable, WithExtraFieldsIdentityTable>(
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDbRepositorySumAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAllAsync<IdentityTable>(e => e.ColumnInt,
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                connection.Truncate<IdentityTable>();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionTruncateAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var task = connection.TruncateAsync<IdentityTable>();
                task.Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Truncate(TableName)

        [TestMethod]
        public void TestSqlConnectionTruncateViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                connection.Truncate(ClassMappedNameCache.Get<IdentityTable>());

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionTruncateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var task = connection.TruncateAsync(ClassMappedNameCache.Get<IdentityTable>());
                task.Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #endregion

        #region Update

        #region Update<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region Update<TEntity>(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, new { entity.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, c => c.Id == entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync<TEntity(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, new { entity.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, c => c.Id == entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region Update(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        item,
                        item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                        item,
                        new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfThePrimaryKeyIsNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfTheFieldsAreNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        #endregion

        #region UpdateAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item,
                    item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    item,
                    new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionUpdateAsyncViaTableNameIfThePrimaryKeyIsNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionUpdateAsyncViaTableNameIfTheFieldsAreNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAll(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAll(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAll(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAllAsync(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAllAsync(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Update each
                var affectedRows = connection.UpdateAllAsync(tables, Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAll(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableNameWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableNameViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableNameViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion

        #region UpdateAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), items).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(), tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion

        #endregion

        #region ExecuteQuery<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(kvpItem =>
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
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQuery<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];");

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
        public void TestSqlConnectionExecuteQueryWhereTheDataReaderColumnsAreLessThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT Id, ColumnBit, ColumnDateTime, ColumnInt FROM [sc].[IdentityTable];");

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
        public void TestSqlConnectionExecuteQueryWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
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
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQueryMultiple.Extract

        #region Extract<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];"))
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
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
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
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
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
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text))
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        #endregion

        #region Extract<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];"))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text))
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value3.First().Id).First(), (ExpandoObject)value3.First());
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteQueryMultipleAsync.ExtractAsync

        #region ExtractAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncMultipleForExtractAsyncWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];").Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text).Result)
                {
                    // Act
                    var value1 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        #endregion

        #region ExtractAsync<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];").Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text).Result)
                {
                    // Act
                    var value1 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value3.First().Id).First(), (ExpandoObject)value3.First());
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteQueryMultiple.Scalar

        #region Scalar<TResult>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithoutParameters()
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
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithMultipleParameters()
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
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
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

        #region Scalar<object>

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

        #endregion

        #region ExecuteQueryMultipleAsync.ScalarAsync

        #region ScalarAsync<TResult>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithoutParameters()
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
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithMultipleParameters()
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
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
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

        #region ScalarAsync<object>

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
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.ScalarAsync().Result;
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
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.ScalarAsync().Result;
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
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteReader

        [TestMethod]
        public void TestSqlConnectionExecuteReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT TOP (@Top) * FROM [sc].[IdentityTable];", new { Top = 2 }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteReaderAsync

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable];").Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT TOP (@Top) * FROM [sc].[IdentityTable];", new { Top = 2 }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader, connection).AsList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;").Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;").Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                connection.ExecuteScalar("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.ExecuteScalarAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                connection.ExecuteScalar<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteScalar<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
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
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

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
                var result = connection.ExecuteScalarAsync<object>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteScalarTAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteScalarAsync<object>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion
    }
}
