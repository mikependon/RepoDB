using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: unlike the SqlServer counterpart of this file, there is no
    /// "...WithMultipleStatement" test here - ODP.NET's Db2Command does not support multiple
    /// SQL statements in a single command text under any circumstances (a plain, non-PL/SQL-block
    /// statement is rejected outright with ORA-00911 as soon as it hits the separating
    /// semicolon). See Db2DbSetting.IsMultiStatementExecutable (always false for this
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
        public void TestDb2ConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Db2.
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryUpdate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("UPDATE \"CompleteTable\" SET \"ColumnVarchar\" = :ColumnVarchar WHERE \"Id\" = :Id",
                new { ColumnVarchar = "Updated", tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncUpdate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("UPDATE \"CompleteTable\" SET \"ColumnVarchar\" = :ColumnVarchar WHERE \"Id\" = :Id",
                new { ColumnVarchar = "Updated", tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        #endregion
    }
}
