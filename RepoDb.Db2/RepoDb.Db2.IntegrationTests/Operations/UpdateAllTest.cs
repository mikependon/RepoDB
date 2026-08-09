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
    /// <summary>
    /// NOTE: now that Db2DbSetting.IsMultiStatementExecutable is true, every 10-row UpdateAll call
    /// below genuinely batches into a single round trip (10 concatenated "UPDATE ... ;" statements
    /// in one command text, executed via ExecuteNonQuery()) instead of 10 separate round trips.
    /// UpdateAll never needs to read a generated value back (only an aggregate affected-row
    /// count), so unlike InsertAll/MergeAll there's no row-correlation concern here.
    /// </summary>
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
        public void TestDb2ConnectionUpdateAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionUpdateAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionUpdateAllWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionUpdateAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionUpdateAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionUpdateAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionUpdateAllAsyncWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionUpdateAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
