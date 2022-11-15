using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS
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
                .UseSQLite();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = Enumerations.ConversionType.Default
                });
        }

        #region List

        #region Sync

        [TestMethod]
        public void TestSQLiteConnectionQueryListContains()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = connection.Query<SdsNonIdentityCompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionQueryEmptyList()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = connection.Query<SdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSQLiteConnectionQueryAsyncListContains()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = await connection.QueryAsync<SdsNonIdentityCompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestSQLiteConnectionQueryAsyncEmptyList()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryAsync<SdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
