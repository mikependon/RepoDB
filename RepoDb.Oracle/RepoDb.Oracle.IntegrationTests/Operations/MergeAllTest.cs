using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    // NOTE: MergeAll's identity-returning behavior (populating "Id" for newly-inserted rows) requires
    // Oracle 23ai+ - the docker-compose target is 23ai Free, and this is already proven working
    // against a live instance in TransactionTests.cs.
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
        public void TestOracleConnectionMergeAllForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeAllForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeAllForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeAllForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeAllViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAllAsyncForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAllAsyncForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAllAsyncForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAllAsyncForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAllAsyncViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
