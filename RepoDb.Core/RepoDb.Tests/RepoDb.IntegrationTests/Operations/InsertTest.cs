using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void TestSqlConnectionInsertWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                item.Id = connection.Insert<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock));

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

        [TestMethod]
        public void TestSqlConnectionInsertAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = connection.InsertAsync<IdentityTable, long>(item, hints: SqlServerTableHints.TabLock).Result);

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

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                        item, hints: SqlServerTableHints.TabLock);
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

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item =>
                {
                    item.Id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                        item, hints: SqlServerTableHints.TabLock).Result;
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

        #endregion
    }
}
