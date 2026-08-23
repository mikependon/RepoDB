using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.IntegrationTests
{
    [TestClass]
    public class TransactionTests
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
        public void TestClickHouseConnectionBeginTransactionThrowsNotSupportedException()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act / Assert
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    connection.EnsureOpen().BeginTransaction());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBeginDbTransactionWithIsolationLevelThrowsNotSupportedException()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act / Assert
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    connection.EnsureOpen().BeginTransaction(System.Data.IsolationLevel.ReadCommitted));
            }
        }
    }
}
