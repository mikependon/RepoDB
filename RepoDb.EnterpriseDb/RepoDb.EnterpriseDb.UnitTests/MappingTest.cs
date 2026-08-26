using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestEnterpriseDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<EDBConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
