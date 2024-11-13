using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
{
    [TestClass]
    public class EnumerableTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = Enumerations.ConversionType.Automatic
                })
                .UseSqlite();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            GlobalConfiguration.Setup(new());
        }

        #region List

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryListContains()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = connection.Query<MdsNonIdentityCompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryEmptyList()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = connection.Query<MdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAsyncListContains()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = await connection.QueryAsync<MdsNonIdentityCompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAsyncEmptyList()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryAsync<MdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
