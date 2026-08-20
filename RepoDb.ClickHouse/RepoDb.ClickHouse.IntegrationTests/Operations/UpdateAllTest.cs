using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Extensions;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
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
        public void TestClickHouseConnectionUpdateAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll<CompleteTable>(tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

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
        public async Task TestClickHouseConnectionUpdateAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync<CompleteTable>(tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

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
        public void TestClickHouseConnectionUpdateAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionUpdateAllViaTableNameAsExpandoObjects()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

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
        public async Task TestClickHouseConnectionUpdateAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionUpdateAllAsyncViaTableNameAsExpandoObjects()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // ClickHouse's ALTER TABLE ... UPDATE mutation reports no meaningful affected-row
                // count via ExecuteNonQuery (it is queued, not applied synchronously) - wait for it instead.
                Helper.WaitForMutations(connection, "CompleteTable");

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
