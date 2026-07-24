using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteTest
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
        public void TestOracleConnectionDelete()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var affectedRows = connection.Delete<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(1, affectedRows);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }
    }
}
