using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.RawSqls
{
    [TestClass]
    public class SqlConnectionOperationsTest
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
