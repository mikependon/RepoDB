using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class QueryTest
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
        public void TestOracleConnectionQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, result.Count());
        }

        [TestMethod]
        public void TestOracleConnectionQueryWithTop()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: exercises the "FETCH FIRST n ROWS ONLY" override.
            var result = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void TestOracleConnectionBatchQuery()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: exercises the "OFFSET x ROWS FETCH NEXT y ROWS ONLY" override.
            var result = connection.BatchQuery<CompleteTable>(page: 1,
                rowsPerBatch: 4,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null);

            // Assert
            Assert.AreEqual(4, result.Count());
        }
    }
}
