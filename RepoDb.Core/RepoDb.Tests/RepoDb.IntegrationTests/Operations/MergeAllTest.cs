using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class MergeAllTest
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

        #region MergeAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables,
                    Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll<NonIdentityTable>(tables,
                    Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<IdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region MergeAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(nameof(NonIdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync<NonIdentityTable>(tables,
                    Field.From(nameof(NonIdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<IdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<IdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region MergeAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAllViaDynamicTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaDynamicTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaExpandoObjectTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaExpandoObjectTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Assert.AreEqual(current.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForExpandoObjectNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForExpandoObjectNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
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
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new List<dynamic>
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeAllIfTheKeyFieldIsNotPresent()
        {
            // Setup
            var tables = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAll(ClassMappedNameCache.Get<NonKeyedTable>(), tables);
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeAllIfThereIsNoKeyField()
        {
            // Setup
            var tables = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAll(ClassMappedNameCache.Get<NonKeyedTable>(), tables);
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeAllIfThereIsNoKeyFieldAtTheTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAll(ClassMappedNameCache.Get<NonKeyedTable>(), tables);
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region MergeAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaDynamicTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaDynamicTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaExpandoObjectTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaExpandoObjectTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Assert.AreEqual(current.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameWithFieldsForNonIdentityEmptyTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
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
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForExpandoObjectNonIdentityNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity,table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForExpandoObjectNonIdentityNonEmptyTableWithQualifier()
        {
            // Setup
            var tables = Helper.CreateExpandoObjectNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<NonIdentityTable>(), tables);

                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(nameof(NonIdentityTable.Id))).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());
                tables.ForEach(table => Assert.IsNotNull(((dynamic)table).Id));

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(item => item.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
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
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var tables = new List<dynamic>
            {
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 1},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 2},
                new {Id = Guid.NewGuid(),ColumnBit = true,ColumnInt = 3}
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityEmptyTableWithHints()
        {
            // Setup
            var tables = Helper.CreateDynamicNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, mergeAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityTable>());

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAllAsyncIfTheKeyFieldIsNotPresent()
        {
            // Setup
            var tables = Helper.CreateNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAllAsync(ClassMappedNameCache.Get<NonKeyedTable>(), tables).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionAsyncMergeAllIfThereIsNoKeyField()
        {
            // Setup
            var tables = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAllAsync(ClassMappedNameCache.Get<NonKeyedTable>(), tables).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAllAsyncIfThereIsNoKeyFieldAtTheTable()
        {
            // Setup
            var tables = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAllAsync(ClassMappedNameCache.Get<NonKeyedTable>(), tables).Wait();
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<IdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(item => item.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion
    }
}
