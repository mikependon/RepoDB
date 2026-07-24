using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        [TestMethod]
        public void TestOracleStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestOracleDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestOracleDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
