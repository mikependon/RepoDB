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
        public void TestSqlConnectionCountAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

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
        public void TestSqlConnectionCountAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        #endregion
    }
}
