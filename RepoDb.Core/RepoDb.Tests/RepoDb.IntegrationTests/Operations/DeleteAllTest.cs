using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = await connection.DeleteAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncViaEntityTableNameWithPrimaryKeysAsType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = tables.Select(e => e.Id);
                var result = await connection.DeleteAllAsync<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync<IdentityTable>();

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync<IdentityTable>(hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncWithEntities()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync<IdentityTable>(tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }


        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncWithDifferentGeneric()
        {
            // Setup
            var tables = Helper.CreateNonMappedIdentityTable(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }


        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = await connection.DeleteAllAsync<IdentityTable>(keys);

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
        public async Task TestSqlConnectionDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncViaTableNameWithEntities()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    tables.Select(e => (object)e.Id));

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<NonIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncViaTableNameWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var keys = new object[] { tables.First().Id, tables.Last().Id };
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    keys);

                // Assert
                Assert.AreEqual(2, result);
                Assert.AreEqual(8, connection.CountAll<NonIdentityTable>());
            }
        }

        #endregion
    }
}
