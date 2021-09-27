using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateMdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll<MdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateMdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAllAsync<MdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateMdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<MdsCompleteTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsExpandoObjectViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);
                var tables = Helper.CreateMdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);
                tables.AsList().ForEach(table => Helper.UpdateMdsCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAllAsyncAsExpandoObjectViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);
                var tables = Helper.CreateMdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.UpdateAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #endregion
    }
}
