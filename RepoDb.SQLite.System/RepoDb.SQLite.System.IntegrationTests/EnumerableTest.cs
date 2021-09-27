using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class EnumerableTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            Converter.ConversionType = Enumerations.ConversionType.Automatic;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            Converter.ConversionType = Enumerations.ConversionType.Default;
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
        public void TestSQLiteConnectionQueryAsyncListContains()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = connection.QueryAsync<SdsNonIdentityCompleteTable>(e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionQueryAsyncEmptyList()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryAsync<SdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
