using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Tests the catalog transaction fixture.
    /// </summary>
    [TestClass]
    public class CatalogTransactionTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the owning connection.
        /// </summary>
        [TestMethod]
        public void RetainsTheConnection()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            Assert.AreSame(connection,
                transaction.Connection);
        }

        /// <summary>
        /// Retains the requested isolation level.
        /// </summary>
        [TestMethod]
        public void RetainsTheIsolationLevel()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            Assert.AreEqual(IsolationLevel.Serializable,
                transaction.IsolationLevel);
        }

        /// <summary>
        /// Leaves the connection open after committing.
        /// </summary>
        [TestMethod]
        public void CommitLeavesTheConnectionOpen()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            transaction.Commit();
            Assert.AreEqual(ConnectionState.Open,
                connection.State);
        }

        /// <summary>
        /// Leaves the connection open after rolling back.
        /// </summary>
        [TestMethod]
        public void RollbackLeavesTheConnectionOpen()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            transaction.Rollback();
            Assert.AreEqual(ConnectionState.Open,
                connection.State);
        }

        #endregion
    }
}
