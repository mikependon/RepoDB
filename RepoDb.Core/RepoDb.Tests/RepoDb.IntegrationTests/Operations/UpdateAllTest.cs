using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;

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
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables, 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }));

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAll<NonIdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<NonIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<NonIdentityTable>(tables, 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" })).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
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

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<NonIdentityTable>(tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                tables.ForEach(item =>
                {
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<NonIdentityTable>(tables,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item =>
                    Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAll(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaDataEntitiesViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
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
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items);

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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    batchSize: 1);

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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
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
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion

        #region UpdateAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items).Result;

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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    batchSize: 1).Result;

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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
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

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables,
                    Field.From(new[] { "ColumnFloat", "ColumnNVarChar" }), 1).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllAsyncViaDataEntitiesViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var items = tables.Select(item => new
                {
                    item.Id,
                    item.ColumnDateTime,
                    item.ColumnDateTime2,
                    item.ColumnFloat,
                    item.ColumnNVarChar,
                    ColumnBit = false,
                    ColumnInt = item.ColumnInt * 100,
                    ColumnDecimal = item.ColumnDecimal * 100
                });

                // Act
                var affectedRows = connection.UpdateAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    items,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
            }
        }

        #endregion
    }
}
