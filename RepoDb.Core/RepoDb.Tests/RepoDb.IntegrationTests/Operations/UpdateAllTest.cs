using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateAllTest
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

        #region UpdateAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaEntityViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaEntityViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync(tables,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(tables,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(tables,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(table =>
                {
                    table.ColumnBit = false;
                    table.ColumnInt = table.ColumnInt * 100;
                    table.ColumnDecimal = table.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = await connection.UpdateAllAsync<NonIdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                tables.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region UpdateAll(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDynamicViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaExpandoObjectViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = Helper.CreateExpandoObjectNonIdentityTables(tables.Count);
                for (var i = 0; i < tables.Count; i++)
                {
                    ((IDictionary<string, object>)items[i])["Id"] = tables[i].Id;
                }

                // Act
                var affectedRows = connection.UpdateAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(e => e.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaExpandoObjectViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = Helper.CreateExpandoObjectNonIdentityTables(tables.Count);
                for (var i = 0; i < tables.Count; i++)
                {
                    ((IDictionary<string, object>)items[i])["Id"] = tables[i].Id;
                }

                // Act
                var affectedRows = connection.UpdateAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(e => e.Id == current.Id);
                    Assert.AreEqual(entity.ColumnBit, current.ColumnBit);
                    Assert.AreEqual(entity.ColumnInt, current.ColumnInt);
                    Assert.AreEqual(entity.ColumnDecimal, current.ColumnDecimal);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    batchSize: 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
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

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion

        #region UpdateAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDynamicViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaExpandoObjectViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = Helper.CreateExpandoObjectNonIdentityTables(tables.Count);
                for (var i = 0; i < tables.Count; i++)
                {
                    ((IDictionary<string, object>)items[i])["Id"] = tables[i].Id;
                }

                // Act
                var affectedRows = await connection.UpdateAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(e => e.Id == current.Id);
                    Helper.AssertMembersEquality(entity, table);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaExpandoObjectViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = Helper.CreateExpandoObjectNonIdentityTables(tables.Count);
                for (var i = 0; i < tables.Count; i++)
                {
                    ((IDictionary<string, object>)items[i])["Id"] = tables[i].Id;
                }

                // Act
                var affectedRows = await connection.UpdateAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var current = (dynamic)table;
                    var entity = queryAllResult.First(e => e.Id == current.Id);
                    Assert.AreEqual(entity.ColumnBit, current.ColumnBit);
                    Assert.AreEqual(entity.ColumnInt, current.ColumnInt);
                    Assert.AreEqual(entity.ColumnDecimal, current.ColumnDecimal);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    batchSize: 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiers()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameViaQualifiersWithSingleBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnFloat), nameof(NonIdentityTable.ColumnNVarChar)), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(table => new
                {
                    table.Id,
                    table.ColumnDateTime,
                    table.ColumnDateTime2,
                    table.ColumnFloat,
                    table.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                }).AsList();

                // Act
                var affectedRows = await connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var queryAllResult = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryAllResult.Count());
                items.ForEach(table =>
                {
                    var entity = queryAllResult.First(e => e.Id == table.Id);
                    Helper.AssertPropertiesEquality(table, entity);
                });
            }
        }

        #endregion
    }
}
