using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqlServer.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqlServerBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestSqlServerDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestSqlServerDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
