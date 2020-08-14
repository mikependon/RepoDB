using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class TruncateTest
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

        #region Truncate<TEntity>

        [TestMethod]
        public void TestSqlConnectionTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                connection.Truncate<IdentityTable>();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionTruncateAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var task = connection.TruncateAsync<IdentityTable>();
                task.Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region Truncate(TableName)

        [TestMethod]
        public void TestSqlConnectionTruncateViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                connection.Truncate(ClassMappedNameCache.Get<IdentityTable>());

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region TruncateAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionTruncateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var task = connection.TruncateAsync(ClassMappedNameCache.Get<IdentityTable>());
                task.Wait();

                // Act
                var result = connection.CountAll<IdentityTable>();

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion
    }
}
