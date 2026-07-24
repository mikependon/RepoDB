using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    /// <summary>
    /// Also exercises OracleStatementBuilder's RETURNING/DBMS_SQL.RETURN_RESULT wrapping (see
    /// InsertTest), plus Oracle's own extra restriction that a MERGE RETURNING clause requires
    /// 12.2+ and exactly one affected row.
    /// </summary>
    [TestClass]
    public class MergeTest
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
        public void TestOracleConnectionMergeForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Merge<CompleteTable>(table);

            // Assert
            Assert.IsTrue(System.Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionMergeForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            table.ColumnVarchar = "Merged";

            // Act
            connection.Merge<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            var queryResult = connection.Query<CompleteTable>(table.Id).First();
            Assert.AreEqual("Merged", queryResult.ColumnVarchar);
        }
    }
}
