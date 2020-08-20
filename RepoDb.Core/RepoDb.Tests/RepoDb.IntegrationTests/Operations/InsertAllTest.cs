using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class InsertAllTest
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

        #region InsertAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(), tables);

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
        public void TestSqlConnectionInsertAllForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

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
                connection.InsertAll<IdentityTable>(tables, 1);

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
                connection.InsertAll<NonIdentityTable>(tables);

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
                connection.InsertAll<NonIdentityTable>(tables, 1);

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
        public void TestSqlConnectionInsertAllForIdentityTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables, hints: SqlServerTableHints.TabLock);

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

        #region InsertAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<WithExtraFieldsIdentityTable>(tables);

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
                connection.InsertAll<WithExtraFieldsIdentityTable>(tables, 1);

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
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();

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
        public void TestSqlConnectionInsertAllAsyncForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync<IdentityTable>(tables).Wait();

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
                connection.InsertAllAsync<IdentityTable>(tables, 1).Wait();

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
                connection.InsertAllAsync<NonIdentityTable>(tables).Wait();

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
                connection.InsertAllAsync<NonIdentityTable>(tables, 1).Wait();

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
        public void TestSqlConnectionInsertAllAsyncForIdentityTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync<IdentityTable>(tables, hints: SqlServerTableHints.TabLock).Wait();

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

        #region InsertAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync<WithExtraFieldsIdentityTable>(tables).Wait();

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
                connection.InsertAllAsync<WithExtraFieldsIdentityTable>(tables, 1).Wait();

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
        public void TestSqlConnectionInsertAllForIdentityTableViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<object>(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

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

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables, hints: SqlServerTableHints.TabLock);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region InsertAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync<object>(ClassMappedNameCache.Get<IdentityTable>(), tables).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

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

        [TestMethod]
        public void TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables, hints: SqlServerTableHints.TabLock).Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion
    }
}
