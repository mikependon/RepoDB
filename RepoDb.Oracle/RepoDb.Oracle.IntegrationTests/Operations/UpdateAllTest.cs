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

        #region Sync

        [TestMethod]
        public void TestOracleConnectionUpdateAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table =>
            {
                table.ColumnVarchar = $"Updated-{table.Id}";
                table.ColumnInt = table.ColumnInt + 1;
            });

            // Act
            var result = connection.UpdateAll<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionUpdateAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table =>
            {
                table.ColumnVarchar = $"Updated-{table.Id}";
                table.ColumnInt = table.ColumnInt + 1;
            });

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.UpdateAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);

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
        public void TestOracleConnectionUpdateAllWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Updated-{table.Id}");

            // Act
            var result = connection.UpdateAll<CompleteTable>(tables, qualifiers);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionUpdateAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Updated-{table.Id}");

            // Act
            var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionUpdateAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table =>
            {
                table.ColumnVarchar = $"Updated-{table.Id}";
                table.ColumnInt = table.ColumnInt + 1;
            });

            // Act
            var result = await connection.UpdateAllAsync<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table =>
            {
                table.ColumnVarchar = $"Updated-{table.Id}";
                table.ColumnInt = table.ColumnInt + 1;
            });

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.UpdateAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);

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
        public async Task TestOracleConnectionUpdateAllAsyncWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Updated-{table.Id}");

            // Act
            var result = await connection.UpdateAllAsync<CompleteTable>(tables, qualifiers);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Updated-{table.Id}");

            // Act
            var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion
    }
}
