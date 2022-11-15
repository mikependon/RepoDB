using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests
{
    [TestClass]
    public class AnonymousTest
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

        #region Helpers

        private dynamic CreateIdentityTableTypeDef()
        {
            return new
            {
                Id = default(long),
                RowGuid = default(Guid),
                ColumnBit = default(bool?),
                ColumnDateTime = default(DateTime?),
                ColumnDateTime2 = default(DateTime?),
                ColumnDecimal = default(decimal?),
                ColumnFloat = default(float?),
                ColumnInt = default(int?),
                ColumnNVarChar = default(string)
            };
        }

        #endregion

        #region ExecuteQuery

        #region Sync

        [TestMethod]
        public void TestExecuteQueryForAnonymous() =>
            TestExecuteQueryForAnonymousTrigger(CreateIdentityTableTypeDef());

        private void TestExecuteQueryForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = connection.ExecuteQuery<TAnonymous>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestExecuteQueryAsyncForAnonymous() =>
            await TestExecuteQueryAsyncForAnonymousTrigger(CreateIdentityTableTypeDef());

        private async Task TestExecuteQueryAsyncForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = await connection.ExecuteQueryAsync<TAnonymous>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Sync

        [TestMethod]
        public void TestQueryForAnonymous() =>
            TestQueryForAnonymousTrigger(CreateIdentityTableTypeDef());

        private void TestQueryForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = connection.Query<TAnonymous>("[sc].[IdentityTable]",
                    what: (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestQueryAsyncForAnonymous() =>
            await TestQueryAsyncForAnonymousTrigger(CreateIdentityTableTypeDef());

        private async Task TestQueryAsyncForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = await connection.QueryAsync<TAnonymous>("[sc].[IdentityTable]",
                    what: (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #endregion

        #region QueryAll

        #region Sync

        [TestMethod]
        public void TestQueryAllForAnonymous() =>
            TestQueryAllForAnonymousTrigger(CreateIdentityTableTypeDef());

        private void TestQueryAllForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = connection.QueryAll<TAnonymous>("[sc].[IdentityTable]");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestQueryAllAsyncForAnonymous() =>
            await TestQueryAllAsyncForAnonymousTrigger(CreateIdentityTableTypeDef());

        private async Task TestQueryAllAsyncForAnonymousTrigger<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                var result = await connection.QueryAllAsync<TAnonymous>("[sc].[IdentityTable]");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
