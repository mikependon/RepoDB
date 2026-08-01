using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    /// <summary>
    /// This is the highest-risk test in the suite: it exercises OracleStatementBuilder's
    /// DECLARE/BEGIN/RETURNING-INTO/DBMS_SQL.RETURN_RESULT wrapping used to surface the
    /// generated identity value through RepoDb.Core's ExecuteScalar()-based Insert pipeline.
    /// Run this first against a real Oracle instance before trusting anything else in this
    /// provider that relies on identity retrieval (Insert, Merge).
    /// </summary>
    [TestClass]
    public class InsertTest
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

        // NOTE: unlike RepoDb.SqlServer.IntegrationTests, this project has only one model
        // (CompleteTable, always identity) - there is no IdentityCompleteTable/NonIdentityCompleteTable
        // split, so there's no "ForNonIdentity" counterpart to any of the tests below.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestOracleConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Insert<CompleteTable>(table);

            // Assert
            Assert.IsTrue(System.Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Assert.AreEqual(1, queryResult?.Count());
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public void TestOracleConnectionInsertForIdentityWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.Insert<CompleteTable>(table);

                // Assert
                Assert.IsTrue(System.Convert.ToInt64(result) > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAsync<CompleteTable>(table);

            // Assert
            Assert.IsTrue(System.Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Assert.AreEqual(1, queryResult?.Count());
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public async Task TestOracleConnectionInsertAsyncForIdentityWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.InsertAsync<CompleteTable>(table);

                // Assert
                Assert.IsTrue(System.Convert.ToInt64(result) > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion

        #endregion

        #region TableName

        // NOTE: RepoDb.SqlServer.IntegrationTests.Operations.InsertTest also exercises "AsDynamic" and
        // "AsExpandoObject" entity sources here. This project's shared Helper (which this workstream does
        // not own) only exposes Helper.CreateCompleteTables (typed CompleteTable) - there's no
        // CreateCompleteTablesAsDynamics/AsExpandoObjects equivalent, so those variants are intentionally
        // skipped rather than inventing untested dynamic/ExpandoObject construction against Oracle.

        #region Sync

        [TestMethod]
        public void TestOracleConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.IsTrue(System.Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Assert.AreEqual(1, queryResult?.Count());
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionInsertAsyncViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.IsTrue(System.Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Assert.AreEqual(1, queryResult?.Count());
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestOracleConnectionInsertWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Oracle - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Insert<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}
