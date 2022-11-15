using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                connection.InsertAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
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
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
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
                connection.InsertAll<IdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                connection.InsertAll<NonIdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                connection.InsertAll<IdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                connection.InsertAll<WithExtraFieldsIdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region InsertAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<NonIdentityTable>(tables);

                // Act
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<NonIdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region InsertAll<TEntity>(Extra Fields)

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<WithExtraFieldsIdentityTable>(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllWithSizePerBatchEqualsToOneAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<WithExtraFieldsIdentityTable>(tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(r => r.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
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
                connection.InsertAll<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaExpandoOjectTableName()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                tables.ForEach(table => Assert.IsTrue(((dynamic)table).Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var currentItem = (dynamic)table;
                    var entity = result.FirstOrDefault(item => item.Id == currentItem.Id);
                    Helper.AssertPropertiesEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaExpandoOjectTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                tables.ForEach(table => Assert.IsTrue(((dynamic)table).Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var currentItem = (dynamic)table;
                    var entity = result.FirstOrDefault(item => item.Id == currentItem.Id);
                    Assert.AreEqual(currentItem.RowGuid, entity.RowGuid);
                    Assert.AreEqual(currentItem.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.Item1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.Item1,
                    1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    1);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Item1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Item1,
                    1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new List<dynamic>
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.RowGuid == table.RowGuid);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    hints: SqlServerTableHints.TabLock);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region InsertAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaExpandoOjectTableName()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                tables.ForEach(table => Assert.IsTrue(((dynamic)table).Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var currentItem = (dynamic)table;
                    var entity = result.FirstOrDefault(item => item.Id == currentItem.Id);
                    Helper.AssertPropertiesEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaExpandoOjectTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync<object>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                tables.ForEach(table => Assert.IsTrue(((dynamic)table).Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var currentItem = (dynamic)table;
                    var entity = result.FirstOrDefault(item => item.Id == currentItem.Id);
                    Assert.AreEqual(currentItem.RowGuid, entity.RowGuid);
                    Assert.AreEqual(currentItem.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.RowGuid, entity.RowGuid);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    1);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.Item1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.Item1,
                    1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableName()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    1);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Item1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<NonIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncWithSizePerBatchEqualsToOneForNonIdentityTableViaTableNameWithLimitedColumns()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTablesWithLimitedColumns(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Item1,
                    1,
                    fields: tables.Item2);

                // Act
                var result = connection.QueryAll<NonIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Item1.Count, result.Count);
                tables.Item1.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithIncompleteProperties()
        {
            // Setup
            var tables = new List<dynamic>
            {
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {RowGuid = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(tables.Count(), result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.RowGuid == table.RowGuid);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.InsertAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    hints: SqlServerTableHints.TabLock);

                // Act
                var result = connection.QueryAll<IdentityTable>().AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion
    }
}
