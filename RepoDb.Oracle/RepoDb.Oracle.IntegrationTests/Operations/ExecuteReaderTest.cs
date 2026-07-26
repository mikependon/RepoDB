using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Reflection;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: unlike the SqlServer counterpart of this file, there is no
    /// "...WithMultipleStatements" test here - ODP.NET's OracleCommand does not support multiple
    /// SQL statements in a single command text under any circumstances. See
    /// OracleDbSetting.IsMultiStatementExecutable (always false for this provider) and
    /// ExecuteQueryMultipleTest.cs for a test that documents this limitation explicitly.
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
        public void TestOracleConnectionExecuteReader()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionExecuteReaderAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            using var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionExecuteReaderAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionExecuteReaderAsyncAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            using var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\"");
            var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).ToList();

            // Assert
            Assert.AreEqual(tables.Count, result.Count);
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteReaderAsyncAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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

        #endregion
    }
}
