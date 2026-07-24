using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateTest
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
        public void TestOracleConnectionUpdate()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            table.ColumnVarchar = "Updated";

            // Act
            var affectedRows = connection.Update(table);

            // Assert
            Assert.AreEqual(1, affectedRows);

            var queryResult = connection.Query<CompleteTable>(table.Id).First();
            Assert.AreEqual("Updated", queryResult.ColumnVarchar);
        }
    }
}
