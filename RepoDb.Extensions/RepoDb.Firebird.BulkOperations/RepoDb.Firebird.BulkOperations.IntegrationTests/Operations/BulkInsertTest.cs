using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System.Data;
using System.Linq;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkInsertOperationsTest
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

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var bulkInsertResult = connection.BulkInsert(tables);

            // Assert
            Assert.AreEqual(tables.Count, bulkInsertResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count);
            for (var i = 0; i < tables.Count; i++)
            {
                Helper.AssertPropertiesEquality(tables[i], queryResult[i]);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var bulkInsertResult = connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(tables.Count, bulkInsertResult);
            Assert.IsFalse(tables.Any(e => e.Id <= 0));
            Assert.AreEqual(tables.Select(e => e.Id).Distinct().Count(), tables.Count);

            // Act
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(t =>
            {
                var item = queryResult.First(e => e.Id == t.Id);
                Helper.AssertPropertiesEquality(t, item);
            });
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var bulkInsertResult = connection.BulkInsert(tables);

            // Assert
            Assert.AreEqual(tables.Count, bulkInsertResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkInsertForDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using var dataTable = new DataTable();
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.Id), typeof(long));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.RowGuid), typeof(byte[]));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnBit), typeof(bool));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnDateTime), typeof(System.DateTime));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), typeof(System.DateTime));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnDecimal), typeof(decimal));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnFloat), typeof(double));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnInt), typeof(int));
            dataTable.Columns.Add(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), typeof(string));
            tables.ForEach(t => dataTable.Rows.Add(t.Id, t.RowGuid, t.ColumnBit, t.ColumnDateTime, t.ColumnDateTime2, t.ColumnDecimal, t.ColumnFloat, t.ColumnInt, t.ColumnNVarChar));

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var bulkInsertResult = connection.BulkInsert(nameof(BulkOperationNonIdentityTable), dataTable);

            // Assert
            Assert.AreEqual(tables.Count, bulkInsertResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFirebirdConnectionBulkInsertForEntitiesAsyncWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var bulkInsertResult = await connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(tables.Count, bulkInsertResult);
            Assert.IsFalse(tables.Any(e => e.Id <= 0));

            // Act
            var queryResult = await connection.QueryAllAsync<BulkOperationIdentityTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }
    }
}
