using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class InsertTest
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

        #region Insert<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(), item));

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsert()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert<IdentityTable, long>(item));

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
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
                Assert.IsTrue(id > 0);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

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

        [TestMethod]
        public void TestSqlConnectionInsertWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock));

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
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
                tables.ForEach(item =>
                    item.Id = connection.Insert<WithExtraFieldsIdentityTable, long>(item));

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
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
                tables.ForEach(item =>
                    connection.InsertAsync<IdentityTable, long>(item).Wait());

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
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
                Assert.IsTrue(id > 0);
                Assert.AreEqual(item.Id, id);

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

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

        [TestMethod]
        public void TestSqlConnectionInsertAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.InsertAsync<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock).Wait());

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
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
                tables.ForEach(item =>
                    connection.InsertAsync<WithExtraFieldsIdentityTable, long>(item).Wait());

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        #endregion

        #region Insert(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameAsDynamicEntities()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert(ClassMappedNameCache.Get<IdentityTable>(), (object)item));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
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
                connection.Insert(ClassMappedNameCache.Get<IdentityTable>(), item);

                // Assert
                Assert.IsTrue(item.Id > 0);

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

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
                var id = connection.Insert<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item);

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, id);
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
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(), item);

                // Assert
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                        item, hints: SqlServerTableHints.TabLock));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        #endregion

        #region InsertAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameAsDynamicEntities()
        {
            // Setup
            var tables = Helper.CreateDynamicIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.InsertAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)item).Wait());

                // Assert
                Assert.IsTrue(tables.All(item => item.Id > 0));

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Wait());

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
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
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    item).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

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
                var id = connection.InsertAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(item.Id, id);
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
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(), item).Result;

                // Assert
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<IdentityTable>()));

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Helper.AssertMembersEquality(item, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                    connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                        item, hints: SqlServerTableHints.TabLock).Wait());

                // Act
                var result = connection.QueryAll<IdentityTable>()?.AsList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result[tables.IndexOf(table)]);
                });
            }
        }

        #endregion
    }
}
