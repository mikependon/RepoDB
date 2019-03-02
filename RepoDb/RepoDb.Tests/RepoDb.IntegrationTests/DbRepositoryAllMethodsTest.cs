using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using System.Data.SqlClient;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class DbRepositoryAllMethodsTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Init();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
            }
        }
    }
}
