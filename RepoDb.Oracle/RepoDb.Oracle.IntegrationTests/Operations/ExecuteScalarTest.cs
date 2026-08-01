using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteScalarTest
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
        public void TestOracleConnectionExecuteScalar()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, Convert.ToInt32(result));
        }

        [TestMethod]
        public void TestOracleConnectionExecuteScalarWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, Convert.ToInt32(result));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestOracleConnectionExecuteScalarWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
        }

        [TestMethod]
        public void TestOracleConnectionExecuteScalarWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Oracle.
            var result = connection.ExecuteScalar<string>("SELECT \"ColumnVarchar\" FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().ColumnVarchar, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExecuteScalarAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, Convert.ToInt32(result));
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteScalarAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, Convert.ToInt32(result));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteScalarAsyncWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteScalarAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync<string>("SELECT \"ColumnVarchar\" FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().ColumnVarchar, result);
        }

        #endregion
    }
}
