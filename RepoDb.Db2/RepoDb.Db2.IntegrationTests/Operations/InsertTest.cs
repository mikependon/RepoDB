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
    /// This is the highest-risk test in the suite: it exercises Db2StatementBuilder's
    /// DECLARE/BEGIN/RETURNING-INTO/DBMS_SQL.RETURN_RESULT wrapping used to surface the
    /// generated identity value through RepoDb.Core's ExecuteScalar()-based Insert pipeline.
    /// Run this first against a real Db2 instance before trusting anything else in this
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
        public void TestDb2ConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionInsertForIdentityWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionInsertAsyncForIdentityWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        // skipped rather than inventing untested dynamic/ExpandoObject construction against Db2.

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionInsertAsyncViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionInsertWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Insert<CompleteTable>(table, hints: "NOLOCK"));
        }

        [TestMethod]
        public async Task TestDb2ConnectionInsertAsyncWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            await Assert.ThrowsAsync<System.NotSupportedException>(() =>
                connection.InsertAsync<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}
