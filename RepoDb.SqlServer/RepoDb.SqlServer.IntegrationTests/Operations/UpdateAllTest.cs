using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionUpdateAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionUpdateAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionUpdateAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionUpdateAllViaExpandoObjectTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionUpdateAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionUpdateAllAsyncViaExpandoObjectTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #endregion
    }
}
