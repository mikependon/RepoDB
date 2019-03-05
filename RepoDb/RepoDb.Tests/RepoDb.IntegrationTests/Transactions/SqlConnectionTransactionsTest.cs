using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Transactions
{
    [TestClass]
    public class SqlConnectionTransactionsTest
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
    }
}
