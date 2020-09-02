using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private object CreateIdentityTableTypeDef()
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

        [TestMethod]
        public void TestQueryAllForAnonymous()
        {
            var result = TestQueryAllForAnonymous(CreateIdentityTableTypeDef());
            Assert.IsTrue(result.Any());
        }

        private IEnumerable<TAnonymous> TestQueryAllForAnonymous<TAnonymous>(TAnonymous typeDef)
            where TAnonymous : class
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(tables);

                // Act
                return connection.QueryAll<TAnonymous>();
            }
        }
    }
}
