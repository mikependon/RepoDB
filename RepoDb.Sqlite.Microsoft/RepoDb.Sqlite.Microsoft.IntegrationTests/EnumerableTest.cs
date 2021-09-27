using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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
        public void TestSqLiteConnectionQueryAsyncListContains()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = connection.QueryAsync<MdsNonIdentityCompleteTable>(e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryAsyncEmptyList()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryAsync<MdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
