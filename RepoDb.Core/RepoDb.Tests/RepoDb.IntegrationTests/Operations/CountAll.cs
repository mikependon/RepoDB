using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class CountAllTest
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

        #region CountAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAllAsync<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAll(TableName)

        [TestMethod]
        public void TestSqlConnectionCountViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion

        #region CountAllAsync(TableName)

        [TestMethod]
        public  async Task TestSqlConnectionCountAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion
    }
}
