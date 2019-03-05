using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class BaseRepositoryOperationsTest
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
