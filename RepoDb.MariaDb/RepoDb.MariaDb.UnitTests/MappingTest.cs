using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestMariaDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestMariaDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
