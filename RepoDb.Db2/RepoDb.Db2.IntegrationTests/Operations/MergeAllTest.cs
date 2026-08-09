using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class MergeAllTest
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

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionMergeAllForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.MergeAll<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act
            var result = connection.MergeAll<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act
            var result = connection.MergeAll<CompleteTable>(tables, qualifiers);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAllAsync<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act
            var result = await connection.MergeAllAsync<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act
            var result = await connection.MergeAllAsync<CompleteTable>(tables, qualifiers);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion
    }
}
