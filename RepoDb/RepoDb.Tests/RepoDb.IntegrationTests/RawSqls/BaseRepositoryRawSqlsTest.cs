using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using System.Data.SqlClient;

namespace RepoDb.IntegrationTests.RawSqls
{
    [TestClass]
    public class BaseRepositoryRawSqlsTest
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
