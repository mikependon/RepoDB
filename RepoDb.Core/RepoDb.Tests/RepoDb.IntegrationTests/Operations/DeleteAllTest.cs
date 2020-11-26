using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteAllTest
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

        #region DeleteAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaEntityTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaEntityTableNameWithDifferentGeneric()
        {
            // Setup
            var tables = Helper.CreateNonMappedIdentityTable(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaEntityTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaEntityTableNameWithPrimaryKeysAsType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = tables.Select(e => e.Id);
                var result = connection.DeleteAll<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

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

        [TestMethod]
        public void TestSqlConnectionDeleteAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll<IdentityTable>(hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAll<IdentityTable>(keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithPrimaryKeysAsType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = tables.Select(e => e.Id);
                var result = connection.DeleteAllAsync<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

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

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync<IdentityTable>(hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync<IdentityTable>(tables).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }


        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncWithDifferentGeneric()
        {
            // Setup
            var tables = Helper.CreateNonMappedIdentityTable(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }


        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAllAsync<IdentityTable>(keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<IdentityTable>());
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

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Select(e => (object)e.Id));

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<NonIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllViaTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAll(ClassMappedNameCache.Get<NonIdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<NonIdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaTableName()
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

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Select(e => (object)e.Id)).Result;

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<NonIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllAsyncViaTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    keys).Result;

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<NonIdentityTable>());
            }
        }

        #endregion
    }
}
