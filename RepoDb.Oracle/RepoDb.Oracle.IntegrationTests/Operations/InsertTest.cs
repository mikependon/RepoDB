using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;

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

        [TestMethod]
        public void TestOracleConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Insert<CompleteTable>(table);

            // Assert: the identity value must have come back through the RETURNING/implicit
            // result set mechanism, not stayed at its CLR default.
            Assert.IsTrue(System.Convert.ToInt64(result) > 0, "The generated identity was not " +
                "returned - the DBMS_SQL.RETURN_RESULT mechanism in OracleStatementBuilder may not " +
                "behave as expected on this Oracle version/ODP.NET combination.");
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Assert.AreEqual(1, queryResult?.Count());
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }
    }
}
