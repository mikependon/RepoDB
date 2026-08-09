using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using RepoDb.Reflection;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: this file previously claimed (an assumption inherited from the Oracle provider this
    /// project was originally templated from, never verified against a live Db2 instance) that
    /// ODP.NET's Db2Command does not support multiple SQL statements in a single command text
    /// under any circumstances. That turned out to be wrong - see ExecuteQueryMultipleTest.cs and
    /// Db2DbSetting.IsMultiStatementExecutable (now true). The "...WithMultipleStatements" tests
    /// below mirror the SqlServer counterpart of this file.
    /// </summary>
    [TestClass]
    public class ExecuteReaderTest
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
        public void TestDb2ConnectionExecuteReader()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
            while (reader.Read())
            {
                // Act
                var id = reader.GetInt32(0);
                var columnInt = reader.GetInt32(1);
                var columnDate = reader.GetDateTime(2);
                var table = tables.FirstOrDefault(e => e.Id == id);

                // Assert
                Assert.IsNotNull(table);
                Assert.AreEqual(columnInt, table.ColumnInt);
                Assert.AreEqual(columnDate, table.ColumnDate);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteReaderWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                using var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
                while (reader.Read())
                {
                    // Act
                    var id = reader.GetInt32(0);
                    var columnInt = reader.GetInt32(1);
                    var columnDate = reader.GetDateTime(2);
                    var table = tables.FirstOrDefault(e => e.Id == id);

                    // Assert
                    Assert.IsNotNull(table);
                    Assert.AreEqual(columnInt, table.ColumnInt);
                    Assert.AreEqual(columnDate, table.ColumnDate);
                }
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteReaderAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteReaderAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: quoted, mixed-case identifiers so the resulting dynamic property names line up
            // exactly with the declared column names.
            using var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnVarchar\", \"ColumnInt\" FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            foreach (var item in result)
            {
                var row = (IDictionary<string, object>)item;
                var table = tables.First(e => e.Id == System.Convert.ToInt32(row["Id"]));
                Assert.AreEqual(table.ColumnVarchar, row["ColumnVarchar"]);
                Assert.AreEqual(table.ColumnInt, System.Convert.ToInt32(row["ColumnInt"]));
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteReaderWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = connection.ExecuteReader(
                "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"; " +
                "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
            do
            {
                while (reader.Read())
                {
                    // Act
                    var id = reader.GetInt32(0);
                    var columnInt = reader.GetInt32(1);
                    var columnDate = reader.GetDateTime(2);
                    var table = tables.FirstOrDefault(e => e.Id == id);

                    // Assert
                    Assert.IsNotNull(table);
                    Assert.AreEqual(columnInt, table.ColumnInt);
                    Assert.AreEqual(columnDate, table.ColumnDate);
                }
            } while (reader.NextResult());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
            while (reader.Read())
            {
                // Act
                var id = reader.GetInt32(0);
                var columnInt = reader.GetInt32(1);
                var columnDate = reader.GetDateTime(2);
                var table = tables.FirstOrDefault(e => e.Id == id);

                // Assert
                Assert.IsNotNull(table);
                Assert.AreEqual(columnInt, table.ColumnInt);
                Assert.AreEqual(columnDate, table.ColumnDate);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteReaderAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                using var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
                while (reader.Read())
                {
                    // Act
                    var id = reader.GetInt32(0);
                    var columnInt = reader.GetInt32(1);
                    var columnDate = reader.GetDateTime(2);
                    var table = tables.FirstOrDefault(e => e.Id == id);

                    // Assert
                    Assert.IsNotNull(table);
                    Assert.AreEqual(columnInt, table.ColumnInt);
                    Assert.AreEqual(columnDate, table.ColumnDate);
                }
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteReaderAsyncAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteReaderAsyncAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnVarchar\", \"ColumnInt\" FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            foreach (var item in result)
            {
                var row = (IDictionary<string, object>)item;
                var table = tables.First(e => e.Id == System.Convert.ToInt32(row["Id"]));
                Assert.AreEqual(table.ColumnVarchar, row["ColumnVarchar"]);
                Assert.AreEqual(table.ColumnInt, System.Convert.ToInt32(row["ColumnInt"]));
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteReaderAsyncWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var reader = await connection.ExecuteReaderAsync(
                "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"; " +
                "SELECT \"Id\", \"ColumnInt\", \"ColumnDate\" FROM \"CompleteTable\"");
            do
            {
                while (reader.Read())
                {
                    // Act
                    var id = reader.GetInt32(0);
                    var columnInt = reader.GetInt32(1);
                    var columnDate = reader.GetDateTime(2);
                    var table = tables.FirstOrDefault(e => e.Id == id);

                    // Assert
                    Assert.IsNotNull(table);
                    Assert.AreEqual(columnInt, table.ColumnInt);
                    Assert.AreEqual(columnDate, table.ColumnDate);
                }
            } while (reader.NextResult());
        }

        #endregion
    }
}
