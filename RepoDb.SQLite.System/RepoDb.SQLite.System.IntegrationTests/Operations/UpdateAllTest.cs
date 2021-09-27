using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionUpdateAll()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

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
        public void TestSqLiteConnectionUpdateAllViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<SdsCompleteTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsExpandoObjectViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(10, connection);
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsyncViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsyncAsExpandoObjectViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(10, connection);
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.UpdateAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #endregion
    }
}
