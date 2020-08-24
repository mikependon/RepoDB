using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class BatchQueryTest
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

        #region BatchQuery<TEntity>

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaEntityTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                Assert.AreEqual(tables.ElementAt(2).ColumnNVarChar, result.ElementAt(0).ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryViaEntityTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameViaQueryFields()
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
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameViaQueryGroup()
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
                var result = connection.BatchQuery<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<IdentityTable>(page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null);

                // Assert (2)
                Assert.AreEqual(tables.ElementAt(2).ColumnNVarChar, result.ElementAt(0).ColumnNVarChar);
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(tableName: ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(tableName: ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithExtraFieldsViaQueryFields()
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
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(tableName: ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryViaEntityTableNameWithExtraFieldsViaQueryGroup()
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
                var result = connection.BatchQuery<WithExtraFieldsIdentityTable>(tableName: ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithWhereForFirstBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithWhereForFirstBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithWhereForSecondBatchInAscendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithWhereForSecondBatchInDescendingOrder()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(20);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(tableName: ClassMappedNameCache.Get<IdentityTable>(),
                    page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (2)
                Assert.AreEqual(tables.ElementAt(2).ColumnNVarChar, result.ElementAt(0).ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameViaQueryFields()
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
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameViaQueryGroup()
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
                var result = connection.BatchQueryAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<IdentityTable>(page: 0,
                    rowsPerBatch: 4,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: new { ColumnInt = 3 },
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)),
                    commandTimeout: 0,
                    transaction: null,
                    trace: null,
                    statementBuilder: null).Result;

                // Assert (2)
                Assert.AreEqual(tables.ElementAt(2).ColumnNVarChar, result.ElementAt(0).ColumnNVarChar);
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithExtraFieldsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithExtraFieldsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 3);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithExtraFieldsViaQueryFields()
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
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
        public void TestSqlConnectionBatchQueryAsyncViaEntityTableNameWithExtraFieldsViaQueryGroup()
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
                var result = connection.BatchQueryAsync<WithExtraFieldsIdentityTable>(ClassMappedNameCache.Get<WithExtraFieldsIdentityTable>(),
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
    }
}
