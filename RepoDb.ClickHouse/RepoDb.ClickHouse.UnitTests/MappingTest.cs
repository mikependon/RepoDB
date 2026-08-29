using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        [TestMethod]
        public void TestClickHouseStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestClickHouseDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestClickHouseDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
