using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: unlike the SqlServer counterpart of this file, there is no
    /// "...WithMultipleStatement" test here - ODP.NET's OracleCommand does not support multiple
    /// SQL statements in a single command text under any circumstances (a plain, non-PL/SQL-block
    /// statement is rejected outright with ORA-00911 as soon as it hits the separating
    /// semicolon). See OracleDbSetting.IsMultiStatementExecutable (always false for this
    /// provider) and ExecuteQueryMultipleTest.cs for a test that documents this limitation
    /// explicitly.
    /// </summary>
    [TestClass]
    public class ExecuteNonQueryTest
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
        public void TestOracleConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Oracle.
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionExecuteNonQueryUpdate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("UPDATE \"CompleteTable\" SET \"ColumnVarchar\" = :ColumnVarchar WHERE \"Id\" = :Id",
                new { ColumnVarchar = "Updated", tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        #endregion
    }
}
